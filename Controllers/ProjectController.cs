using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetworkTopologyVisitingCard.Data;
using NetworkTopologyVisitingCard.Models;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkTopologyVisitingCard.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            var projects = await _context.Projects.ToListAsync();
            return View(projects);
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Technologies,ImageUrl,Author")] Project project)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                
                project.CreatedDate = DateTime.UtcNow;
                project.OwnerId = userId; // Устанавливаем владельца проекта
                
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            // Проверяем, является ли пользователь владельцем или админом
            if (!await IsProjectOwnerAsync(project))
            {
                return Forbid(); // 403 Forbidden
            }

            return View(project);
        }

        // POST: Projects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,CreatedDate,Technologies,ImageUrl,Author")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            var existingProject = await _context.Projects.FindAsync(id);
            if (existingProject == null)
            {
                return NotFound();
            }

            // Проверяем права собственности
            if (!await IsProjectOwnerAsync(existingProject))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    existingProject.Title = project.Title;
                    existingProject.Description = project.Description;
                    existingProject.Technologies = project.Technologies;
                    existingProject.ImageUrl = project.ImageUrl;
                    existingProject.Author = project.Author;
                    
                    _context.Update(existingProject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            // Проверяем права собственности
            if (!await IsProjectOwnerAsync(project))
            {
                return Forbid();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                // Проверяем права собственности
                if (!await IsProjectOwnerAsync(project))
                {
                    return Forbid();
                }

                _context.Projects.Remove(project);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }

        // Проверяет, является ли текущий пользователь владельцем проекта или админом
        private async Task<bool> IsProjectOwnerAsync(Project project)
        {
            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");
            
            return project.OwnerId == currentUserId || isAdmin;
        }
    }
}
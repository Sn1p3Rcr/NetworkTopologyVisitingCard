using Microsoft.AspNetCore.Identity;
using NetworkTopologyVisitingCard.Models;

namespace NetworkTopologyVisitingCard.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Создаем роли
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Создаем администратора
            var adminUser = new ApplicationUser
            {
                UserName = "admin@topology.com",
                Email = "admin@topology.com",
                FullName = "Administrator",
                EmailConfirmed = true,
                RegistrationDate = DateTime.UtcNow
            };

            string adminPassword = "Admin123!";
            var user = await userManager.FindByEmailAsync(adminUser.Email);

            if (user == null)
            {
                var createPowerUser = await userManager.CreateAsync(adminUser, adminPassword);
                if (createPowerUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Добавляем тестовые проекты, если их нет
            if (!context.Projects.Any())
            {
                context.Projects.AddRange(
                    new Project
                    {
                        Title = "Визуализатор сетевой топологии предприятия",
                        Description = "Интерактивный инструмент для проектирования корпоративных сетей",
                        CreatedDate = DateTime.UtcNow.AddDays(-10),
                        Technologies = "React, TypeScript, D3.js, ASP.NET Core",
                        ImageUrl = "https://via.placeholder.com/400x250/3498db/ffffff?text=Enterprise+Network",
                        Author = "Алексей Петров"
                    },
                    new Project
                    {
                        Title = "Облачная сеть мониторинга",
                        Description = "Система для визуализации и мониторинга облачных инфраструктур",
                        CreatedDate = DateTime.UtcNow.AddDays(-5),
                        Technologies = "Vue.js, WebSocket, C#, SQL Server",
                        ImageUrl = "https://via.placeholder.com/400x250/2ecc71/ffffff?text=Cloud+Network",
                        Author = "Мария Сидорова"
                    }
                );
                await context.SaveChangesAsync();
            }

            if (!context.Technologies.Any())
            {
                context.Technologies.AddRange(
                    new Technology { Name = "React", Description = "UI-библиотека", Category = "Frontend" },
                    new Technology { Name = "ASP.NET Core", Description = "Backend-фреймворк", Category = "Backend" },
                    new Technology { Name = "D3.js", Description = "Визуализация графов", Category = "Visualization" }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
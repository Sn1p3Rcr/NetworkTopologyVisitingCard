using System;
using System.ComponentModel.DataAnnotations;

namespace NetworkTopologyVisitingCard.Models
{
    public class Project
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Название проекта обязательно")]
        [StringLength(100, ErrorMessage = "Название не должно превышать 100 символов")]
        public string Title { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Описание проекта обязательно")]
        [StringLength(500, ErrorMessage = "Описание не должно превышать 500 символов")]
        public string Description { get; set; } = string.Empty;
        
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }
        
        public string Technologies { get; set; } = string.Empty;
        
        public string ImageUrl { get; set; } = string.Empty;
        
        public string Author { get; set; } = string.Empty;

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<NetworkTopology> NetworkTopologies { get; set; } = new List<NetworkTopology>();
        public ICollection<ProjectTechnology> ProjectTechnologies { get; set; } = new List<ProjectTechnology>();
    }
}
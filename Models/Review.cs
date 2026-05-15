using System;
using System.ComponentModel.DataAnnotations;

namespace NetworkTopologyVisitingCard.Models
{
    public class Review
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Имя автора обязательно")]
        public string AuthorName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Текст отзыва обязателен")]
        [StringLength(1000, ErrorMessage = "Отзыв не должен превышать 1000 символов")]
        public string Content { get; set; } = string.Empty;
        
        [DataType(DataType.Date)]
        public DateTime ReviewDate { get; set; }
        
        [Range(1, 5, ErrorMessage = "Рейтинг должен быть от 1 до 5")]
        public int Rating { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;
    }
}
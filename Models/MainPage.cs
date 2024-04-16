using System.ComponentModel.DataAnnotations;

namespace MoonCafe.Models
{
    public class MainPage
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Title Required")]
        [StringLength(50, ErrorMessage = "Title cannot exceed 50 characters.")]
        [MinLength(10, ErrorMessage = "Title cannot be less than 10 frames.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Title Required")]
        [StringLength(50, ErrorMessage = "Title cannot exceed 50 characters.")]
        [MinLength(10, ErrorMessage = "Title cannot be less than 10 frames.")]
        public string TitleT { get; set; }
        [Required(ErrorMessage = "Description Required")]
        [StringLength(350, ErrorMessage = "Description cannot exceed 350 characters.")]
        [MinLength(10, ErrorMessage = "Description cannot be less than 10 frames.")]
        public string MainPageDescription { get; set; }
    }
}

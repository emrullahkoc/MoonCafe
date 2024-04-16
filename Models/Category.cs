using System.ComponentModel.DataAnnotations;

namespace MoonCafe.Models
{
    public class Category
    {
        public Category()
        {
            Activities = new HashSet<Activity>();
        }
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Category Name Required")]
        [StringLength(30, ErrorMessage = "Category Name cannot exceed 30 characters.")]
        [MinLength(6, ErrorMessage = "Category Name cannot be less than 6 frames.")]
        public string CategoryName { get; set; }

        [Required(ErrorMessage = "Category Description Required")]
        [StringLength(600, ErrorMessage = "Category Description cannot exceed 600 characters.")]
        [MinLength(10, ErrorMessage = "Category Description cannot be less than 10 frames.")]
        public string CategoryDescription { get; set; }
        public string? CategoryImageUrl { get; set; }
        public bool CategoryStatus { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }
    }
}

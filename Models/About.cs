using System.ComponentModel.DataAnnotations;

namespace MoonCafe.Models
{
    public class About
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "About Title Required")]
        [StringLength(40, ErrorMessage = "About Title cannot exceed 40 characters.")]
        [MinLength(10, ErrorMessage = "About Title cannot be less than 10 frames.")]
        public string AboutTitle { get; set; }
        [Required(ErrorMessage = "About Details Required")]
        [MinLength(20, ErrorMessage = "About Details cannot be less than 20 frames.")]
        public string AboutDetails { get; set; }
        public string? AboutImageUrl { get; set; }
        [Required(ErrorMessage = "Map Location Required")]
        public string AboutMapLocation { get; set; }
        [StringLength(100, ErrorMessage = "Address cannot exceed 100 characters.")]
        [MinLength(10, ErrorMessage = "Address cannot be less than 10 frames.")]
        [Required(ErrorMessage = "Address Required")]
        public string Address { get; set; }
        [StringLength(11, ErrorMessage = "Telephone cannot exceed 11 characters.")]
        [MinLength(11, ErrorMessage = "Telephone cannot be less than 11 frames.")]
        [Required(ErrorMessage = "Telephone Required")]
        public string Telephone { get; set; }
        [Required(ErrorMessage = "Mail Required")]
        [StringLength(30, ErrorMessage = "Mail cannot exceed 30 characters.")]
        [MinLength(10, ErrorMessage = "Mail cannot be less than 10 frames.")]
        public string Mail { get; set; }
    }
}

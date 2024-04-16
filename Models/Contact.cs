using System.ComponentModel.DataAnnotations;

namespace MoonCafe.Models
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name Required")]
        [StringLength(30, ErrorMessage = "Name cannot exceed 30 characters.")]
        [MinLength(6, ErrorMessage = "Name cannot be less than 6 frames.")]
        public string ContactFullName { get; set; }
        [Required(ErrorMessage = "Mail Required")]
        [StringLength(30, ErrorMessage = "Mail cannot exceed 30 characters.")]
        [MinLength(10, ErrorMessage = "Mail cannot be less than 10 frames.")]
        public string ContactMail { get; set; }
        [Required(ErrorMessage = "Message Required")]
        [StringLength(300, ErrorMessage = "Message cannot exceed 300 characters.")]
        [MinLength(25, ErrorMessage = "Message cannot be less than 25 frames.")]
        public string ContactMessage { get; set; }
        public string? ContactImageUrl { get; set; }
        public DateTime ContactDate { get; set; }
        public bool ContactStatus { get; set; }
    }
}

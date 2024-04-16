using System.ComponentModel.DataAnnotations;

namespace MoonCafe.Models
{
    public class User
    {
        public User()
        {
            Tickets = new HashSet<Ticket>();
        }
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "User Name Required")]
        [StringLength(30, ErrorMessage = "User Name cannot exceed 30 characters.")]
        [MinLength(6, ErrorMessage = "User Name cannot be less than 6 frames.")]
        public string UserFullName { get; set; }
        [Required(ErrorMessage = "User Mail Required")]
        [StringLength(30, ErrorMessage = "User Mail cannot exceed 30 characters.")]
        [MinLength(10, ErrorMessage = "User Mail cannot be less than 10 frames.")]
        public string UserEmail { get; set; }
        [Required(ErrorMessage = "User Password Required")]
        [StringLength(30, ErrorMessage = "User Password cannot exceed 30 characters.")]
        public string UserPassword { get; set; }
        public string? UserImageUrl { get; set; }
        [Required(ErrorMessage = "Role Required")]
        public string Role { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool UserStatus { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}

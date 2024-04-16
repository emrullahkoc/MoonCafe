using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace MoonCafe.Models
{
    public class Activity
    {
        public Activity()
        {
            Tickets = new HashSet<Ticket>();
        }
        [Key]
        public int Id { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }
        [Required]
        public int ArtistId { get; set; }
        [ForeignKey("ArtistId")]
        public virtual Artist? Artist { get; set; }
        [Required(ErrorMessage = "Activity Name Required")]
        [StringLength(30, ErrorMessage = "Activity Name cannot exceed 30 characters.")]
        [MinLength(6, ErrorMessage = "Activity Name cannot be less than 6 frames.")]
        public string ActivityName { get; set; }
        [Required(ErrorMessage = "Activity Description Required")]
        [StringLength(600, ErrorMessage = "Activity Description cannot exceed 600 characters.")]
        [MinLength(10, ErrorMessage = "Activity Description cannot be less than 10 frames.")]
        public string ActivityDescription { get; set; }
        [Required(ErrorMessage = "Activity Date Time Required")]
        public DateTime ActivityDate { get; set; }
        [Required(ErrorMessage = "Activity Price Required")]
        public decimal ActivityPrice { get; set; }
        public string? ActivityImageUrl { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool ActivityStatus { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}

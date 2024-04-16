using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MoonCafe.Models
{
    public class Ticket
    {
        [Key] public Guid Id { get; set; }

        [Required]
        public int ActivityId { get; set; }
        [ForeignKey("ActivityId")]
        public virtual Activity? Activity { get; set; }


        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
        public int NumberPeople { get; set; }
        public DateTime? ControlDateTime { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool TicketStatus { get; set; }
    }
}

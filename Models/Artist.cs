using System.ComponentModel.DataAnnotations;

namespace MoonCafe.Models
{
    public class Artist
    {
        public Artist()
        {
            Activities = new HashSet<Activity>();
        }
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Artist Name Required")]
        [StringLength(30, ErrorMessage = "Artist Name cannot exceed 30 characters.")]
        [MinLength(6, ErrorMessage = "Artist Name cannot be less than 6 frames.")]
        public string ArtistName { get; set; }
        [Required(ErrorMessage = "Artist Description Required")]
        [StringLength(1000, ErrorMessage = "Artist Description cannot exceed 1000 characters.")]
        [MinLength(10, ErrorMessage = "Artist Description cannot be less than 10 frames.")]
        public string ArtistDescription { get; set; }
        [Required(ErrorMessage = "Artist Birth Date Required")]
        public DateTime ArtistBirthDate { get; set; }
        public string? ArtistImageUrl { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool ArtistStatus { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }
    }
}

namespace MoonCafe.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int ArtistCount { get; set; }
        public int CategoryCount { get; set; }
        public int ActivityCount { get; set; }
        public IEnumerable<Activity> Activities { get; set; }
    }
}

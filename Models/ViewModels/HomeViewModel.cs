namespace MoonCafe.Models.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Activity> Activities { get; set; }
        public IEnumerable<Activity> FinishedActivities { get; set; }
        public IEnumerable<Artist> Artists { get; set; }
        public IEnumerable<Contact> Contacts { get; set; }
        public string Title { get; set; }
        public string TitleT { get; set; }
        public string MainPageDescription { get; set; }
    }
}

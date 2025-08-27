namespace ExpiryApp.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int OverdueCount { get; set; }
        public int Within7DaysCount { get; set; }
        public int Within30DaysCount { get; set; }
    }
}

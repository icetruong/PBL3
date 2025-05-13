namespace HoldEvent.Models
{
    public class RevenueReportEventViewModel
    {
        public string EventName { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal NetRevenue { get; set; }
        public int SoldQuantity { get; set; }
    }
}

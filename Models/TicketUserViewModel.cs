namespace HoldEvent.Models
{
    public class TicketUserViewModel
    {
        public string NameEvent { get; set; } = null!;
        public string NameVenue { get; set; } = null!;
        public int? Quantity { get; set; }
        public int? TicketType { get; set; }
        public string? Address { get; set; }
        public int? Status { get; set; }
        public decimal? Price { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}

using HoldEvent.Models.Entities;

namespace HoldEvent.Models
{
    public class TicketEventViewModel
    {
        public string TicketId { get; set; } = null!;

        public string? EventId { get; set; }

        public string? CommissionId { get; set; }

        public int? TicketType { get; set; }

        public decimal? Price { get; set; }

        public int? TotalQuantity { get; set; }

        public List<Event> Events { get; set; }
    }
}

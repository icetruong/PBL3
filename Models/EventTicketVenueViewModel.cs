using HoldEvent.Models.Entities;

namespace HoldEvent.Models
{
    public class EventTicketVenueViewModel
    {
        public string EventId { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public bool? IsPublic { get; set; }

        public int? EventStatus { get; set; }

        public string NamePlace { get; set; } = null!;

        public decimal? PricePlace { get; set; }

        public byte[]? Image { get; set; }
    }
}

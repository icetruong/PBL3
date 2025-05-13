using HoldEvent.Models.Entities;

namespace HoldEvent.Models
{
    public class EventBookingViewModel
    {
        public string EventId { get; set; } = null!;

        public string? VenueId { get; set; }

        public List<Venue?> Venues { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public bool? IsPublic { get; set; }

        public int? EventStatus { get; set; }
    }
}

using HoldEvent.Models.Entities;

namespace HoldEvent.Models
{
    public class EventTicketOrganizerVenueViewModel
    {
        public string EventId { get; set; } = null!;

        public string? NameOrganizer { get; set; }

        public string Email { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string? NameVenue { get; set; }

        public string? DescriptionVenue { get; set; }

        public string? Address { get; set; }

        public string NameEvent { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public List<Ticket> tickets { get; set; }

        public byte[]? Image { get; set; }
    }
}

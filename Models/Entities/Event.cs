using System;
using System.Collections.Generic;

namespace HoldEvent.Models.Entities;

public partial class Event
{
    public string EventId { get; set; } = null!;

    public string? OrganizerId { get; set; }

    public string? VenueId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public bool? IsPublic { get; set; }

    public int? EventStatus { get; set; }

    public byte[]? Image { get; set; }

    public virtual User? Organizer { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual Venue? Venue { get; set; }
}
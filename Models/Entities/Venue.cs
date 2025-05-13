using System;
using System.Collections.Generic;

namespace HoldEvent.Models.Entities;

public partial class Venue
{
    public string VenueId { get; set; } = null!;

    public string? OwnPlaceId { get; set; }

    public string? CommissionId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? Capacity { get; set; }

    public string? Address { get; set; }

    public decimal? Price { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Commission? Commission { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual User? OwnPlace { get; set; }
}

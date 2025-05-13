using HoldEvent.Models.Entities;
using System;
using System.Collections.Generic;

namespace HoldEvent.Models.Entities;

public partial class Booking
{
    public string BookingId { get; set; } = null!;

    public string? VenueId { get; set; }

    public string? OrganizerId { get; set; }

    public DateTime? DateStart { get; set; }

    public DateTime? DateEnd { get; set; }

    public DateTime? BookingDate { get; set; }

    public int? BookingStatus { get; set; }

    public decimal? Deposit { get; set; }

    public string? PaymentId { get; set; }

    public virtual User? Organizer { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual Venue? Venue { get; set; }
}

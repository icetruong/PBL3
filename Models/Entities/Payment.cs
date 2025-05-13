using System;
using System.Collections.Generic;

namespace HoldEvent.Models.Entities;

public partial class Payment
{
    public string PaymentId { get; set; } = null!;

    public int? Method { get; set; }

    public int? PaymentStatus { get; set; }

    public decimal? Price { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}

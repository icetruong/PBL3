using System;
using System.Collections.Generic;

namespace HoldEvent.Models.Entities;

public partial class Commission
{
    public string CommissionId { get; set; } = null!;

    public int? CommissionType { get; set; }

    public decimal? Percentage { get; set; }

    public DateTime? CreateAtDay { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual ICollection<Venue> Venues { get; set; } = new List<Venue>();
}

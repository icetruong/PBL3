using System;
using System.Collections.Generic;

namespace HoldEvent.Models.Entities;

public partial class Ticket
{
    public string TicketId { get; set; } = null!;

    public string? EventId { get; set; }

    public string? CommissionId { get; set; }

    public int? TicketType { get; set; }

    public decimal? Price { get; set; }

    public int? TotalQuantity { get; set; }

    public int? SoldQuantity { get; set; }

    public virtual Commission? Commission { get; set; }

    public virtual Event? Event { get; set; }

    public virtual ICollection<TicketOfUser> TicketOfUsers { get; set; } = new List<TicketOfUser>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}

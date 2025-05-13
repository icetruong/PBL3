using System;
using System.Collections.Generic;

namespace HoldEvent.Models.Entities;

public partial class TicketOfUser
{
    public string UserId { get; set; } = null!;

    public string TicketId { get; set; } = null!;

    public string TransactionId { get; set; } = null!;

    public int? Status { get; set; }

    public virtual Ticket Ticket { get; set; } = null!;

    public virtual Transaction Transaction { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

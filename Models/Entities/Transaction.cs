using System;
using System.Collections.Generic;

namespace HoldEvent.Models.Entities;

public partial class Transaction
{
    public string TransactionId { get; set; } = null!;

    public string? PaymentId { get; set; }

    public string? TicketId { get; set; }

    public string? UserId { get; set; }

    public int? Quantity { get; set; }

    public DateTime? TransactionDate { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual Ticket? Ticket { get; set; }

    public virtual ICollection<TicketOfUser> TicketOfUsers { get; set; } = new List<TicketOfUser>();

    public virtual User? User { get; set; }
}

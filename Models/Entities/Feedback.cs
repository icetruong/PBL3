using System;
using System.Collections.Generic;

namespace HoldEvent.Models.Entities;

public partial class Feedback
{
    public string FeedbackId { get; set; } = null!;

    public string? UserId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public int? FeedbackState { get; set; }

    public DateTime? CreateAtDay { get; set; }

    public virtual ICollection<Support> Supports { get; set; } = new List<Support>();

    public virtual User? User { get; set; }
}

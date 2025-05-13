using System;
using System.Collections.Generic;

namespace HoldEvent.Models.Entities;

public partial class Support
{
    public string SupportId { get; set; } = null!;

    public string? AdminId { get; set; }

    public string? FeedbackId { get; set; }

    public string? Content { get; set; }

    public DateTime? CreateAtDay { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual Feedback? Feedback { get; set; }
}

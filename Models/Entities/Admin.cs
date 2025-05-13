using System;
using System.Collections.Generic;

namespace HoldEvent.Models.Entities;

public partial class Admin
{
    public string AdminId { get; set; } = null!;

    public string? UserId { get; set; }

    public virtual ICollection<Support> Supports { get; set; } = new List<Support>();

    public virtual User? User { get; set; }
}

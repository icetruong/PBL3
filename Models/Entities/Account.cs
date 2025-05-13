using System;
using System.Collections.Generic;

namespace HoldEvent.Models.Entities;

public partial class Account
{
    public string AccountId { get; set; } = null!;

    public string UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime? CreateAtDay { get; set; }

    public virtual User? User { get; set; }
}

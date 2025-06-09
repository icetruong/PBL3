using System;
using System.Collections.Generic;

namespace HoldEvent.Models.Entities;

public partial class User
{
    public string UserId { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? Address { get; set; }

    public DateTime? DayOfBirth { get; set; }

    public bool? Gender { get; set; }

    public string? Role { get; set; }

    public byte[]? Avatar { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<Admin> Admins { get; set; } = new List<Admin>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<TicketOfUser> TicketOfUsers { get; set; } = new List<TicketOfUser>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual ICollection<Venue> Venues { get; set; } = new List<Venue>();
}
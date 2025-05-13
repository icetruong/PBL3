using System;
using System.Collections.Generic;
using HoldEvent.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HoldEvent.Models.Entities;

public partial class Pbl3Context : DbContext
{
    public Pbl3Context()
    {
    }

    public Pbl3Context(DbContextOptions<Pbl3Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Commission> Commissions { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Support> Supports { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<TicketOfUser> TicketOfUsers { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Venue> Venues { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server = LAPTOP-51U89A0M; Database = PBL3; Trusted_Connection=True; TrustServerCertificate = True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Accounts__349DA58659DE27D0");

            entity.Property(e => e.AccountId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("AccountID");
            entity.Property(e => e.CreateAtDay).HasColumnType("datetime");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UserId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("UserID");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Accounts__UserID__02FC7413");
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Admins__719FE4E869E05C35");

            entity.Property(e => e.AdminId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("AdminID");
            entity.Property(e => e.UserId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Admins)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Admins__UserID__3B75D760");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Bookings__73951ACD0C8EB41A");

            entity.Property(e => e.BookingId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("BookingID");
            entity.Property(e => e.BookingDate).HasColumnType("datetime");
            entity.Property(e => e.DateEnd).HasColumnType("datetime");
            entity.Property(e => e.DateStart).HasColumnType("datetime");
            entity.Property(e => e.Deposit).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.OrganizerId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("OrganizerID");
            entity.Property(e => e.PaymentId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("PaymentID");
            entity.Property(e => e.VenueId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("VenueID");

            entity.HasOne(d => d.Organizer).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.OrganizerId)
                .HasConstraintName("FK__Bookings__Organi__4F7CD00D");

            entity.HasOne(d => d.Payment).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("FK__Bookings__Paymen__14270015");

            entity.HasOne(d => d.Venue).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.VenueId)
                .HasConstraintName("FK__Bookings__VenueI__4E88ABD4");
        });

        modelBuilder.Entity<Commission>(entity =>
        {
            entity.HasKey(e => e.CommissionId).HasName("PK__Commissi__6C2C8CEC77ECDACC");

            entity.Property(e => e.CommissionId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CommissionID");
            entity.Property(e => e.CreateAtDay).HasColumnType("datetime");
            entity.Property(e => e.Percentage).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Events__7944C870E3415401");

            entity.Property(e => e.EventId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("EventID");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.IsPublic).HasDefaultValue(true);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.OrganizerId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("OrganizerID");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.VenueId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("VenueID");

            entity.HasOne(d => d.Organizer).WithMany(p => p.Events)
                .HasForeignKey(d => d.OrganizerId)
                .HasConstraintName("FK__Events__Organize__52593CB8");

            entity.HasOne(d => d.Venue).WithMany(p => p.Events)
                .HasForeignKey(d => d.VenueId)
                .HasConstraintName("FK__Events__VenueID__534D60F1");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__6A4BEDF60A826A78");

            entity.Property(e => e.FeedbackId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("FeedbackID");
            entity.Property(e => e.CreateAtDay).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(50);
            entity.Property(e => e.UserId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Feedbacks__UserI__08B54D69");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A583C627687");

            entity.Property(e => e.PaymentId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("PaymentID");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Support>(entity =>
        {
            entity.HasKey(e => e.SupportId).HasName("PK__Supports__D82DBC6C16CC7999");

            entity.Property(e => e.SupportId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SupportID");
            entity.Property(e => e.AdminId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("AdminID");
            entity.Property(e => e.CreateAtDay).HasColumnType("datetime");
            entity.Property(e => e.FeedbackId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("FeedbackID");

            entity.HasOne(d => d.Admin).WithMany(p => p.Supports)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("FK__Supports__AdminI__0B91BA14");

            entity.HasOne(d => d.Feedback).WithMany(p => p.Supports)
                .HasForeignKey(d => d.FeedbackId)
                .HasConstraintName("FK__Supports__Feedba__0C85DE4D");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PK__Tickets__712CC627A88E1FD1");

            entity.Property(e => e.TicketId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("TicketID");
            entity.Property(e => e.CommissionId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CommissionID");
            entity.Property(e => e.EventId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("EventID");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Commission).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.CommissionId)
                .HasConstraintName("FK__Tickets__Commiss__5812160E");

            entity.HasOne(d => d.Event).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.EventId)
                .HasConstraintName("FK__Tickets__EventID__571DF1D5");
        });

        modelBuilder.Entity<TicketOfUser>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.TicketId, e.TransactionId }).HasName("PK__TicketOf__3ACF43F4283D2591");

            entity.ToTable("TicketOfUser");

            entity.Property(e => e.UserId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("UserID");
            entity.Property(e => e.TicketId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("TicketID");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("TransactionID");

            entity.HasOne(d => d.Ticket).WithMany(p => p.TicketOfUsers)
                .HasForeignKey(d => d.TicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TicketOfU__Ticke__71D1E811");

            entity.HasOne(d => d.Transaction).WithMany(p => p.TicketOfUsers)
                .HasForeignKey(d => d.TransactionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TicketOfU__Trans__70DDC3D8");

            entity.HasOne(d => d.User).WithMany(p => p.TicketOfUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TicketOfU__UserI__72C60C4A");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Transact__55433A4B0528F52F");

            entity.Property(e => e.TransactionId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("TransactionID");
            entity.Property(e => e.PaymentId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("PaymentID");
            entity.Property(e => e.TicketId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("TicketID");
            entity.Property(e => e.TransactionDate).HasColumnType("datetime");
            entity.Property(e => e.UserId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("UserID");

            entity.HasOne(d => d.Payment).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("FK__Transacti__Payme__5CD6CB2B");

            entity.HasOne(d => d.Ticket).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.TicketId)
                .HasConstraintName("FK__Transacti__Ticke__5DCAEF64");

            entity.HasOne(d => d.User).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Transacti__UserI__5EBF139D");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC8B976F6E");

            entity.HasIndex(e => e.Email, "UQ_Users_Email").IsUnique();

            entity.Property(e => e.UserId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("UserID");
            entity.Property(e => e.Address).HasMaxLength(50);
            entity.Property(e => e.DayOfBirth).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FullName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber).HasMaxLength(10);
            entity.Property(e => e.Role)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("User");
        });

        modelBuilder.Entity<Venue>(entity =>
        {
            entity.HasKey(e => e.VenueId).HasName("PK__Venues__3C57E5D2EE660A0A");

            entity.Property(e => e.VenueId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("VenueID");
            entity.Property(e => e.Address).HasMaxLength(50);
            entity.Property(e => e.CommissionId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CommissionID");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.OwnPlaceId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("OwnPlaceID");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Commission).WithMany(p => p.Venues)
                .HasForeignKey(d => d.CommissionId)
                .HasConstraintName("FK__Venues__Commissi__4AB81AF0");

            entity.HasOne(d => d.OwnPlace).WithMany(p => p.Venues)
                .HasForeignKey(d => d.OwnPlaceId)
                .HasConstraintName("FK__Venues__OwnPlace__49C3F6B7");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

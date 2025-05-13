namespace HoldEvent.Models
{
    public class VenueBookingViewModel
    {
        public string VenueId { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int? Capacity { get; set; }

        public string? Address { get; set; }

        public decimal? Price { get; set; }

        public DateTime? DateStart { get; set; }

        public DateTime? DateEnd { get; set; }

        public decimal? Deposit { get; set; }

        public string? PaymentId { get; set; }

        public int? Method { get; set; }

        public decimal? PricePayment { get; set; }
    }
}

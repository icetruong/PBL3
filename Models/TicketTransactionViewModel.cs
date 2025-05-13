using HoldEvent.Models.Entities;

namespace HoldEvent.Models
{
    public class TicketTransactionViewModel
    {
        public string? TicketId { get; set; }
        public string EventId { get; set; }

        public string? NameEvent { get; set; }

        public int? TicketType { get; set; }

        public int? Quantity { get; set; }

        public decimal? TotalPrice { get; set; }

        public string? PaymentId { get; set; }

        public int? Method { get; set; }

        public decimal? PricePayment { get; set; }

        public List<Ticket> tickets { get; set; }
    }
}

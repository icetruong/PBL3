namespace HoldEvent.Models
{
    public class ListTicketViewModel
    {
        public string TicketId { get; set; }

        public int? TicketType { get; set; }

        public decimal? Price { get; set; }

        public int? TotalQuantity { get; set; }

        public int? SoldQuantity { get; set; }

        public string Name { get; set; }
    }
}

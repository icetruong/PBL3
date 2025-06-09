using System.Reflection;

namespace HoldEvent.Models
{
    public class AdminViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalEvents { get; set; }
        public int TotalTicketsSold { get; set; }
        public List<EventInfo> UpcomingEvents { get; set; } = new();
    }

    public class EventInfo
    {
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string VenueName { get; set; }
        public int TotalQuantity { get; set; }
        public int SoldQuantity { get; set; }
    }
}

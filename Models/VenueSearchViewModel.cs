using HoldEvent.Models.Entities;

namespace HoldEvent.Models
{
    public class VenueSearchViewModel
    {
        public String? Search { get; set; }
        public String? Select { get; set; }
        public List<Venue> Venues { get; set; }
    }
}

namespace HoldEvent.Models
{
    public class FeedbackSupportViewModel
    {
        public string FeedbackId { get; set; }

        public string? UserId { get; set; }

        public string? Title { get; set; }

        public string? ContentFeedback { get; set; }

        public DateTime? CreateAtDayFeedback { get; set; }
        public string SupportId { get; set; }

        public string? AdminId { get; set; }

        public string? ContentSupport { get; set; }

        public DateTime? CreateAtDaySupport { get; set; }
    }
}

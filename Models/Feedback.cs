namespace MemberFeedbackApi.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Message { get; set; }
        public int Rating { get; set; } // 1-5
        public bool IsApproved { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
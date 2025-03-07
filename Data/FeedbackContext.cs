using Microsoft.EntityFrameworkCore;

namespace MemberFeedbackApi.Data
{
    public class FeedbackContext : DbContext
    {
        public FeedbackContext(DbContextOptions<FeedbackContext> options) : base(options) { }
        public DbSet<MemberFeedbackApi.Models.Feedback> Feedbacks { get; set; }
    }
}
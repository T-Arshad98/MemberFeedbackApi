using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MemberFeedbackApi.Data;
using MemberFeedbackApi.Models;

namespace MemberFeedbackApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly FeedbackContext _context;

        public FeedbackController(FeedbackContext context)
        {
            _context = context;
        }

        // Public: Get approved feedback
        [HttpGet]
        [ResponseCache(Duration = 300)] // Cache for 5 minutes
        public async Task<ActionResult<IEnumerable<Feedback>>> GetApprovedFeedback()
        {
            return await _context.Feedbacks.Where(f => f.IsApproved).ToListAsync();
        }

        // Public: Get stats (average rating)
        [HttpGet("stats")]
        [ResponseCache(Duration = 300)]
        public async Task<ActionResult<object>> GetStats()
        {
            var approved = await _context.Feedbacks.Where(f => f.IsApproved).ToListAsync();
            return new { AverageRating = approved.Any() ? approved.Average(f => f.Rating) : 0, Total = approved.Count };
        }

        // Public: Submit feedback
        [HttpPost]
        public async Task<ActionResult<Feedback>> PostFeedback(Feedback feedback)
        {
            feedback.IsApproved = false; // Pending approval
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetApprovedFeedback), new { id = feedback.Id }, feedback);
        }

        // Admin: Get all feedback (approved + pending)
        [HttpGet("admin")]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetAllFeedback()
        {
            if (!IsAdmin()) return Unauthorized();
            return await _context.Feedbacks.ToListAsync();
        }

        // Admin: Approve feedback
        [HttpPut("admin/approve/{id}")]
        public async Task<IActionResult> ApproveFeedback(int id)
        {
            if (!IsAdmin()) return Unauthorized();
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null) return NotFound();
            feedback.IsApproved = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Admin: Reject feedback
        [HttpDelete("admin/reject/{id}")]
        public async Task<IActionResult> RejectFeedback(int id)
        {
            if (!IsAdmin()) return Unauthorized();
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null) return NotFound();
            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Admin: Login
        [HttpPost("admin/login")]
        public IActionResult Login([FromBody] string password)
        {
            if (password == "secretpassword") // Hardcoded for demo
            {
                Response.Cookies.Append("AdminSession", "secretpassword", new CookieOptions { HttpOnly = true });
                return Ok();
            }
            return Unauthorized();
        }

        private bool IsAdmin()
        {
            return Request.Cookies["AdminSession"] == "secretpassword"; // Simple cookie-based auth
        }
    }
}
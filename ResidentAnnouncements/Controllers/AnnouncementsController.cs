using Microsoft.AspNetCore.Mvc;
using RA_MuniAnnouncements.Application.Interfaces;
using System.Threading.Tasks;

namespace RA_MuniAnnouncements.Api.Controllers
{
    [ApiController]
    [Route("api/announcements/{authorityCode}")]
    public class AnnouncementsController : ControllerBase
    {
        private readonly IAnnouncementService _service;

        public AnnouncementsController(IAnnouncementService service)
        {
            _service = service;
        }

        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> Get(string authorityCode)
        {
            // Validation: empty authority code
            if (string.IsNullOrWhiteSpace(authorityCode))
            {
                return BadRequest(new { message = "Authority code cannot be null or empty." });
            }

            // Call service - let it handle business logic
            var result = await _service.GetNormalizedAnnouncementsAsync(authorityCode);
            
            // If no settings for this authority, return 404 per business contract
            if (result == null)
            {
                return NotFound(new { message = $"Settings for authority '{authorityCode}' not found." });
            }

            return Ok(result);
        }
    }
}
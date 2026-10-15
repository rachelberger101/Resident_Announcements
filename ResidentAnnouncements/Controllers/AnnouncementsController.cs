using Microsoft.AspNetCore.Mvc;
using ResidentAnnouncements.Application.Interfaces;
using System.Threading.Tasks;

namespace ResidentAnnouncements.Api.Controllers
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
            var result = await _service.GetNormalizedAnnouncementsAsync(authorityCode);
            if (result == null)
            {
                return NotFound(new { message = $"Settings for authority '{authorityCode}' not found." });
            }

            return Ok(result);
        }
    }
}
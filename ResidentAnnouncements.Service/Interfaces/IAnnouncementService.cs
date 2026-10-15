using System.Threading.Tasks;
using ResidentAnnouncements.Core.Models;

namespace ResidentAnnouncements.Application.Interfaces
{
    public interface IAnnouncementService
    {
        Task<NormalizedAnnouncementResponse?> GetNormalizedAnnouncementsAsync(string authorityCode);
    }
}
using System.Threading.Tasks;
using RA_MuniAnnouncements.Core.Models;

namespace RA_MuniAnnouncements.Application.Interfaces
{
    public interface IAnnouncementService
    {
        Task<NormalizedAnnouncementResponse?> GetNormalizedAnnouncementsAsync(string authorityCode);
    }
}
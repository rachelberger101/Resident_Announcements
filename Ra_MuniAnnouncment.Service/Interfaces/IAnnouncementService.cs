using RA_MuniAnnouncements.Core.Models;
using System.Threading.Tasks;


namespace RA_MuniAnnouncements.Application.Interfaces
{
    public interface IAnnouncementService
    {
        Task<NormalizedAnnouncementResponse?> GetNormalizedAnnouncementsAsync(string authorityCode);
    }
}
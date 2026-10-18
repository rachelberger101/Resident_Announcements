using System.Collections.Generic;
using System.Threading.Tasks;
using RA_MuniAnnouncements.Core.Models;

namespace RA_MuniAnnouncements.Infrastructure.Repositories
{
    public interface ICmsClient
    {
        Task<List<AnnouncementFromCms>> GetAnnouncementsAsync(string authorityCode);
    }
}
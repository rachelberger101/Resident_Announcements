using System.Collections.Generic;
using System.Threading.Tasks;
using ResidentAnnouncements.Core.Models;

namespace ResidentAnnouncements.Infrastructure.Repositories
{
    public interface ICmsClient
    {
        Task<List<AnnouncementFromCms>> GetAnnouncementsAsync(string authorityCode);
    }
}
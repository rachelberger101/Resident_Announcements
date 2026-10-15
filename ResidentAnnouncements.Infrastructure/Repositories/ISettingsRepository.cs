using System.Threading.Tasks;
using ResidentAnnouncements.Core.Models;

namespace ResidentAnnouncements.Infrastructure.Repositories
{
    public interface ISettingsRepository
    {
        Task<AuthoritySiteSettings?> GetAuthoritySettingsAsync(string authorityCode);
    }
}
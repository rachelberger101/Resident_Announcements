using System.Threading.Tasks;
using RA_MuniAnnouncements.Core.Models;

namespace RA_MuniAnnouncements.Infrastructure.Repositories
{
    public interface ISettingsRepository
    {
        Task<AuthoritySiteSettings?> GetAuthoritySettingsAsync(string authorityCode);
    }
}
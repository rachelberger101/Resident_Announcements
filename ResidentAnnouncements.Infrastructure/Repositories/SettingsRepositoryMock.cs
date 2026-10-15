using ResidentAnnouncements.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResidentAnnouncements.Infrastructure.Repositories
{
    // Simulate MongoDB AuthoritySiteSettings collection
    public class SettingsRepositoryMock : ISettingsRepository
    {
        private static readonly List<AuthoritySiteSettings> _store = new()
        {
            new AuthoritySiteSettings
            {
                AuthorityCode = "6500",
                AuthorityName = "עיריית חדרה",
                EnabledCategories = new List<string> { "roads", "water", "events" },
                CategoryDisplayNames = new Dictionary<string, string>
                {
                    { "roads", "תחבורה וכבישים" },
                    { "water", "מוביוב ומים" },
                    { "events", "אירועים" }
                },
                UrgentCategories = new List<string> { "water" },
                MaxItems = 5
            }
        };

        public Task<AuthoritySiteSettings?> GetAuthoritySettingsAsync(string authorityCode)
        {
            var s = _store.Find(x => x.AuthorityCode == authorityCode);
            return Task.FromResult(s);
        }
    }
}
using RA_MuniAnnouncements.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RA_MuniAnnouncements.Infrastructure.Repositories
{
    /// <summary>
    /// Mock implementation of ISettingsRepository for testing/demo purposes.
    /// Returns in-memory simulated settings data.
    /// </summary>
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
            // In-memory lookup - no exceptions can occur here
            // Returns null if not found (business logic)
            var settings = _store.Find(x => x.AuthorityCode == authorityCode);
            return Task.FromResult(settings);
        }
    }
}
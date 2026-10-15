using ResidentAnnouncements.Application.Interfaces;
using ResidentAnnouncements.Core.Models;
using ResidentAnnouncements.Infrastructure.Repositories;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ResidentAnnouncements.Application.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly ICmsClient _cmsClient;
        private readonly ISettingsRepository _settingsRepository;

        public AnnouncementService(ICmsClient cmsClient, ISettingsRepository settingsRepository)
        {
            _cmsClient = cmsClient;
            _settingsRepository = settingsRepository;
        }

        public async Task<NormalizedAnnouncementResponse?> GetNormalizedAnnouncementsAsync(string authorityCode)
        {
            // fetch settings
            var settings = await _settingsRepository.GetAuthoritySettingsAsync(authorityCode);
            if (settings == null) return null;

            // fetch raw announcements from CMS
            var raw = await _cmsClient.GetAnnouncementsAsync(authorityCode);

            var now = DateTime.UtcNow;

            // filter published & active, and enabled categories
            var filtered = raw
                .Where(a => string.Equals(a.Status, "Published", StringComparison.OrdinalIgnoreCase))
                .Where(a => a.PublishDate <= now && a.ExpiryDate > now)
                .Where(a => settings.EnabledCategories.Contains(a.Category))
                .ToList();

            // normalize
            var normalized = filtered
                .Select(a =>
                {
                    var categoryName = settings.CategoryDisplayNames.TryGetValue(a.Category, out var display) ? display : a.Category;
                    var isUrgent = settings.UrgentCategories.Contains(a.Category);
                    var summary = StripHtml(a.Body);
                    if (summary.Length > 150) summary = summary[..150].TrimEnd() + "...";
                    return new NormalizedAnnouncementItem
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Summary = summary,
                        CategoryName = categoryName,
                        IsUrgent = isUrgent,
                        PublishDate = a.PublishDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                    };
                })
                .OrderByDescending(a => a.IsUrgent) // urgent first (true > false)
                .ThenByDescending(a => DateTime.ParseExact(a.PublishDate, "dd/MM/yyyy", CultureInfo.InvariantCulture))
                .Take(settings.MaxItems)
                .ToList();

            return new NormalizedAnnouncementResponse
            {
                AuthorityCode = settings.AuthorityCode,
                AuthorityName = settings.AuthorityName,
                Announcements = normalized
            };
        }

        private static string StripHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;
            var noTags = Regex.Replace(html, "<.*?>", string.Empty);
            var decoded = System.Net.WebUtility.HtmlDecode(noTags);
            return decoded.Trim().Replace("\n", " ").Replace("\r", " ").Replace("\t", " ");
        }
    }
}
using RA_MuniAnnouncements.Application.Interfaces;
using RA_MuniAnnouncements.Core.Models;
using RA_MuniAnnouncements.Infrastructure.Repositories;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace RA_MuniAnnouncements.Application.Services
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
            // Fetch settings from repository
            // If not found, return null (business logic decision)
            var settings = await _settingsRepository.GetAuthoritySettingsAsync(authorityCode);
            if (settings == null)
            {
                return null;
            }

            // Fetch raw announcements from CMS
            // If unavailable, return empty list or let exception propagate to middleware
            var raw = await _cmsClient.GetAnnouncementsAsync(authorityCode);
            if (raw == null)
            {
                raw = new List<AnnouncementFromCms>();
            }

            var now = DateTime.UtcNow;

            // Filter: published & active, and enabled categories
            var filtered = raw
                .Where(a => string.Equals(a.Status, "Published", StringComparison.OrdinalIgnoreCase))
                .Where(a => a.PublishDate <= now && a.ExpiryDate > now)
                .Where(a => settings.EnabledCategories.Contains(a.Category))
                .ToList();

            // Normalize announcements
            var normalized = filtered
                .Select(a =>
                {
                    var categoryName = settings.CategoryDisplayNames.TryGetValue(a.Category, out var display) 
                        ? display 
                        : a.Category;
                    var isUrgent = settings.UrgentCategories.Contains(a.Category);
                    var summary = StripHtml(a.Body);
                    if (summary.Length > 150)
                    {
                        summary = summary[..150].TrimEnd() + "...";
                    }
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
                .OrderByDescending(a => a.IsUrgent) // Urgent first (true > false)
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
            if (string.IsNullOrWhiteSpace(html))
            {
                return string.Empty;
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var text = doc.DocumentNode.InnerText;
            var decoded = System.Net.WebUtility.HtmlDecode(text);
            return decoded.Trim()
                .Replace("\n", " ")
                .Replace("\r", " ")
                .Replace("\t", " ");
        }
    }
}
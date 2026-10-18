using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RA_MuniAnnouncements.Application.Services;
using RA_MuniAnnouncements.Core.Models;
using RA_MuniAnnouncements.Infrastructure.Repositories;
using Xunit;

namespace RA_MuniAnnouncements.Tests
{
    public class AnnouncementServiceNormalizationTests
    {
        // Simple test fakes to avoid external dependencies
        private class FakeSettingsRepository : ISettingsRepository
        {
            private readonly AuthoritySiteSettings? _settings;
            public FakeSettingsRepository(AuthoritySiteSettings? settings) => _settings = settings;
            public Task<AuthoritySiteSettings?> GetAuthoritySettingsAsync(string authorityCode)
            {
                if (_settings is null) return Task.FromResult<AuthoritySiteSettings?>(null);
                return Task.FromResult(_settings.AuthorityCode == authorityCode ? _settings : null);
            }
        }

        private class FakeCmsClient : ICmsClient
        {
            private readonly List<AnnouncementFromCms> _items;
            public FakeCmsClient(List<AnnouncementFromCms> items) => _items = items;
            public Task<List<AnnouncementFromCms>> GetAnnouncementsAsync(string authorityCode)
            {
                var result = _items.FindAll(a => a.AuthorityCode == authorityCode);
                return Task.FromResult(result);
            }
        }

        [Fact]
        public async Task ActiveAnnouncement_IsIncluded()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var announcement = new AnnouncementFromCms
            {
                Id = "active-1",
                AuthorityCode = "6500",
                Title = "Active message",
                Body = "<p>content</p>",
                Category = "roads",
                Status = "Published",
                PublishDate = now.AddDays(-1),
                ExpiryDate = now.AddDays(1)
            };

            var settings = new AuthoritySiteSettings
            {
                AuthorityCode = "6500",
                AuthorityName = "Test Authority",
                EnabledCategories = new List<string> { "roads", "water" },
                CategoryDisplayNames = new Dictionary<string, string> { { "roads", "Roads" } },
                UrgentCategories = new List<string>(),
                MaxItems = 10
            };

            var cms = new FakeCmsClient(new List<AnnouncementFromCms> { announcement });
            var repo = new FakeSettingsRepository(settings);
            var service = new AnnouncementService(cms, repo);

            // Act
            var result = await service.GetNormalizedAnnouncementsAsync("6500");

            // Assert
            Assert.NotNull(result);
            Assert.Contains(result.Announcements, a => a.Id == "active-1");
        }

        [Fact]
        public async Task ExpiredAnnouncement_IsExcluded()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var expired = new AnnouncementFromCms
            {
                Id = "expired-1",
                AuthorityCode = "6500",
                Title = "Expired message",
                Body = "<p>expired</p>",
                Category = "roads",
                Status = "Published",
                PublishDate = now.AddDays(-10),
                ExpiryDate = now.AddDays(-1) // already expired
            };

            var settings = new AuthoritySiteSettings
            {
                AuthorityCode = "6500",
                AuthorityName = "Test Authority",
                EnabledCategories = new List<string> { "roads" },
                CategoryDisplayNames = new Dictionary<string, string>(),
                UrgentCategories = new List<string>(),
                MaxItems = 10
            };

            var cms = new FakeCmsClient(new List<AnnouncementFromCms> { expired });
            var repo = new FakeSettingsRepository(settings);
            var service = new AnnouncementService(cms, repo);

            // Act
            var result = await service.GetNormalizedAnnouncementsAsync("6500");

            // Assert
            Assert.NotNull(result);
            Assert.DoesNotContain(result.Announcements, a => a.Id == "expired-1");
        }

        [Fact]
        public async Task Announcement_FromInactiveCategory_IsExcluded()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var item = new AnnouncementFromCms
            {
                Id = "inactivecat-1",
                AuthorityCode = "6500",
                Title = "Inactive category",
                Body = "<p>ok</p>",
                Category = "events",
                Status = "Published",
                PublishDate = now.AddDays(-1),
                ExpiryDate = now.AddDays(5)
            };

            var settings = new AuthoritySiteSettings
            {
                AuthorityCode = "6500",
                AuthorityName = "Test Authority",
                EnabledCategories = new List<string> { "roads", "water" }, // 'events' NOT enabled
                CategoryDisplayNames = new Dictionary<string, string>(),
                UrgentCategories = new List<string>(),
                MaxItems = 10
            };

            var cms = new FakeCmsClient(new List<AnnouncementFromCms> { item });
            var repo = new FakeSettingsRepository(settings);
            var service = new AnnouncementService(cms, repo);

            // Act
            var result = await service.GetNormalizedAnnouncementsAsync("6500");

            // Assert
            Assert.NotNull(result);
            Assert.DoesNotContain(result.Announcements, a => a.Id == "inactivecat-1");
        }
    }
}
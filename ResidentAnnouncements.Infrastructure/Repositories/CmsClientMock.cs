using ResidentAnnouncements.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResidentAnnouncements.Infrastructure.Repositories
{
    // Simulate CMS data (hard-coded)
    public class CmsClientMock : ICmsClient
    {
        public Task<List<AnnouncementFromCms>> GetAnnouncementsAsync(string authorityCode)
        {
            var list = new List<AnnouncementFromCms>
            {
                new AnnouncementFromCms
                {
                    Id = "a-1001",
                    AuthorityCode = "6500",
                    Title = "סגירת רחוב הרברט סמואל",
                    Body = "<p>בשל עבודות תשתית ייסגר הרחוב לתנועה</p>",
                    Category = "roads",
                    Status = "Published",
                    PublishDate = DateTime.Parse("2026-10-01T08:00:00Z"),
                    ExpiryDate = DateTime.Parse("2026-10-20T23:59:00Z")
                },
                new AnnouncementFromCms
                {
                    Id = "a-1002",
                    AuthorityCode = "6500",
                    Title = "הפסקת מים לשכונה ג'",
                    Body = "<p>יתכנו הפסקות מים זמניות מ-09:00 עד 15:00</p>",
                    Category = "water",
                    Status = "Published",
                    PublishDate = DateTime.UtcNow.AddDays(-1),
                    ExpiryDate = DateTime.UtcNow.AddDays(3)
                },
                new AnnouncementFromCms
                {
                    Id = "a-1003",
                    AuthorityCode = "6500",
                    Title = "אירוע קהילתי בפארק",
                    Body = "<p>פסטיבל משפחות בפארק העירוני</p>",
                    Category = "events",
                    Status = "Draft",
                    PublishDate = DateTime.UtcNow.AddDays(-2),
                    ExpiryDate = DateTime.UtcNow.AddDays(10)
                }
            };

            // return only those matching authorityCode for demo
            var result = list.FindAll(x => x.AuthorityCode == authorityCode);
            return Task.FromResult(result);
        }
    }
}
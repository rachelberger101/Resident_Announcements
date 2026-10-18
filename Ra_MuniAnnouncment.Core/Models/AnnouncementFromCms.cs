using System;

namespace RA_MuniAnnouncements.Core.Models
{
    public class AnnouncementFromCms
    {
        public string Id { get; set; } = null!;
        public string AuthorityCode { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime PublishDate { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
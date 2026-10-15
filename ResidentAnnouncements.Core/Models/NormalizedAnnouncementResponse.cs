using System.Collections.Generic;

namespace ResidentAnnouncements.Core.Models
{
    public class NormalizedAnnouncementItem
    {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Summary { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public bool IsUrgent { get; set; }
        public string PublishDate { get; set; } = null!;
    }

    public class NormalizedAnnouncementResponse
    {
        public string AuthorityCode { get; set; } = null!;
        public string AuthorityName { get; set; } = null!;
        public List<NormalizedAnnouncementItem> Announcements { get; set; } = new();
    }
}
using System.Collections.Generic;

namespace ResidentAnnouncements.Core.Models
{
    public class AuthoritySiteSettings
    {
        public string AuthorityCode { get; set; } = null!;
        public string AuthorityName { get; set; } = null!;
        public List<string> EnabledCategories { get; set; } = new();
        public Dictionary<string, string> CategoryDisplayNames { get; set; } = new();
        public List<string> UrgentCategories { get; set; } = new();
        public int MaxItems { get; set; } = 5;
    }
}
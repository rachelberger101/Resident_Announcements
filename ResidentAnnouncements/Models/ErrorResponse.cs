namespace RA_MuniAnnouncements.Api.Models
{
    public class ErrorResponse
    {
        public string TraceId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Details { get; set; }
        public int StatusCode { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? Exception { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
    }
}

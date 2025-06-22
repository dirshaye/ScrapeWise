public class ConfigProfile
{
    public int ConfigProfileId { get; set; }
    public string DisplayName { get; set; } = "Default User";
    public string AvatarUrl { get; set; } = "https://www.gravatar.com/avatar/?d=mp";
    public string UserAgent { get; set; } = string.Empty;
    public int DelayBetweenRequests { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }
}

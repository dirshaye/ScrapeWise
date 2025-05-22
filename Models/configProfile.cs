public class ConfigProfile
{
    public int ConfigProfileId { get; set; }
    public string UserAgent { get; set; }
    public int DelayBetweenRequests { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }
}

namespace Common.Settings;

public class CacheSettings
{
    public string ConnectionString { get; set; }
    public int AbsoluteExpirationInSeconds { get; set; }
    public int SlidingExpirationInSeconds { get; set; }
}

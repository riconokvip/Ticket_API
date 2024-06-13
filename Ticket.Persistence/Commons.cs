namespace Ticket.Persistence
{
    public static class CacheSettings
    {
        public static int EXPIRED_TIME = 5;

        public static DistributedCacheEntryOptions CACHE_OPTION = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(EXPIRED_TIME)
        };

        public static string EsKey(string name) => $"es_{name}";
        public static string ResKey(string name) => $"res_{name}";
    }
}

namespace Ticket.Persistence
{
    public static class RedisKeys
    {
        public static string Users(int pageIndex, int pageSize, string textSearch) => $"users_{textSearch}_{pageIndex}_{pageSize}";

        public static string EsUsers = "esusers";
    }
}

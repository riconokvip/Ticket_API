using Nest;

namespace Ticket.Persistence
{
    public interface IEsRepo<T>
    {
        Task<List<T>> GetAll();

        Task<IEnumerable<string>> AddOrUpdateBulk(IEnumerable<T> documents);

        Task<long> RemoveAll();
    }

    public class EsRepo<T> : IEsRepo<T> where T : class
    {
        private readonly IElasticClient _client;
        private readonly string _indexName;

        public EsRepo(IElasticClient client)
        {
            _client = client;
            _indexName = typeof(T).Name.ToLower();
        }

        public async Task<List<T>> GetAll()
        {
            var searchResponse = await _client.SearchAsync<T>(s => s.Index(_indexName).Query(q => q.MatchAll()));
            return searchResponse.IsValid ? searchResponse.Documents.ToList() : default;
        }

        public async Task<IEnumerable<string>> AddOrUpdateBulk(IEnumerable<T> documents)
        {
            var response = await _client.BulkAsync(b => b
                   .Index(_indexName)
                   .UpdateMany(documents, (ud, d) => ud.Doc(d).DocAsUpsert(true))
               );
            return response.Items.Select(x => x.Id);
        }

        public async Task<long> RemoveAll()
        {
            var response = await _client.DeleteByQueryAsync<T>(d => d.Index(_indexName).Query(q => q.MatchAll()));
            return response.Deleted;
        }
    }
}

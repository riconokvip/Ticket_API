using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace Ticket.Persistence
{
    public static class PersistenceInstallers
    {
        public static void AddEsServices(this IServiceCollection services, IConfiguration configuration)
        {
            var credentials = configuration.GetSection("Elastic");
            var pool = new SingleNodeConnectionPool(new Uri("https://localhost:9200"));
            var settings = new ConnectionSettings(pool)
                .BasicAuthentication(credentials["user"], credentials["pass"])
                .ServerCertificateValidationCallback(CertificateValidations.AllowAll);

            services.AddSingleton<IElasticClient>(new ElasticClient(settings));
            services.AddScoped(typeof(IEsRepo<>), typeof(EsRepo<>));
        }
    }
}

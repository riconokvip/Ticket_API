using Elasticsearch.Net;
using Nest;

namespace Ticket.Persistence
{
    public static class PersistenceInstallers
    {
        public static void AddEsServices(this IServiceCollection services, IConfiguration configuration)
        {
            var credentials = configuration.GetSection("Elastic") ??
                throw new Exception("Let's check elastic of config in appsettings.json");

            var pool = new SingleNodeConnectionPool(new Uri(credentials["Host"]));
            var settings = new ConnectionSettings(pool)
                .BasicAuthentication(credentials["User"], credentials["Pass"])
                .ServerCertificateValidationCallback(CertificateValidations.AllowAll);

            services.AddSingleton<IElasticClient>(new ElasticClient(settings));
            services.AddScoped(typeof(IEsRepo<>), typeof(EsRepo<>));
        }

        public static void AddRedisCaches(this IServiceCollection services, IConfiguration configuration)
        {
            var credentials = configuration.GetSection("Redis") ??
                throw new Exception("Let's check redis of config in appsettings.json");

            services.AddStackExchangeRedisCache(options => {
                options.Configuration = credentials["ConnectionStrings"];
            });

            services.AddScoped<IDistributedCacheService, DistributedCacheService>();
        }
    }
}

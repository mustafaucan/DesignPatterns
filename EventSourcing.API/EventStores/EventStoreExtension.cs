using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EventSourcing.API.EventStores
{
    public static class EventStoreExtension
    {
        public static void AddEventStore(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("EventStore");
            var settings = EventStoreClientSettings.Create(connectionString);
            var client = new EventStoreClient(settings);

            services.AddSingleton(client);

            using var logFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddConsole();
            });
            var logger = logFactory.CreateLogger("Startup");
                        
        }
    }
}

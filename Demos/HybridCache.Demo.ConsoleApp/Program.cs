using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using HybridCache.InMemoryRedis.RedisBus;
using HybridCache.Demo.Domain;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace HybridCache.Demo.ConsoleApp
{
    class Program
    {
        static async Task  Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Async(c => c.File("Logs/logs.txt"))
                .WriteTo.Async(c => c.Console())
                .CreateLogger();

            IServiceCollection services = new ServiceCollection();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                //loggingBuilder.AddNLog(nLogConfig);
                loggingBuilder.AddSerilog();
            });

            services.AddInMemoryRedisCacheWithRedisBus(options =>
            {
                options.TopicName = "cache_sync_topic";
                options.InMemory = new InMemoryCacheOptions();
                options.InMemory.CacheName = "Memory1";

                options.Redis=new RedisCacheOptions();
                options.Redis.CacheName = "redis1";
                options.Redis.Host = "127.0.0.1";
                options.Redis.Port = 6379;
                options.Redis.Password = null;
                options.Redis.Database = 2;
                options.Redis.SerializerName = "TextJsonSerializer";

                options.Bus=new HybridCacheBusOptions();
                options.Bus.Redis=new BusWithRedisOptions();
                options.Bus.Redis.Name = "hybrid-cache-redis-bus1";
                options.Bus.Redis.Host = "127.0.0.1";
                options.Bus.Redis.Port = 6379;
                options.Bus.Redis.Password = null;
                options.Bus.Redis.Database = 2;
            });
            services.AddScoped<UserManager>();

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            var userManager = serviceProvider.GetService<UserManager>();

            var userList = await userManager.GetListFromCacheAsync();
            var json = System.Text.Json.JsonSerializer.Serialize(userList);
            Console.WriteLine(json);

            Console.ReadKey();
        }
    }
}

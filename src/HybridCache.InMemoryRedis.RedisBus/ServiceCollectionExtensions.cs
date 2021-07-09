using System;
using EasyCaching.Bus.Redis;
using EasyCaching.Core.Configurations;
using EasyCaching.HybridCache;
using EasyCaching.Redis;
using EasyCaching.Serialization.SystemTextJson.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HybridCache.InMemoryRedis.RedisBus
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加混合缓存（内存+Redis+Redis Bus）
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configurationSection"></param>
        /// <returns></returns>
        public static IServiceCollection AddInMemoryRedisCacheWithRedisBus(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            var memoryCacheName = configurationSection.GetValue<string>("InMemory:CacheName");

            var hybridCacheTopicName = configurationSection.GetValue<string>("TopicName");
            var redisCacheHost = configurationSection.GetValue<string>("Redis:Host");
            var redisCachePort = configurationSection.GetValue<int>("Redis:Port");
            var redisCachePassword = configurationSection.GetValue<string>("Redis:Password");
            var redisCacheSerializerName = configurationSection.GetValue<string>("Redis:SerializerName");
            var redisCacheName = configurationSection.GetValue<string>("Redis:CacheName");
            var redisCacheDatabase = configurationSection.GetValue<int>("Redis:Database");
            
            var redisBusName = configurationSection.GetValue<string>("Bus:Redis:Name");
            var redisBusHost = configurationSection.GetValue<string>("Bus:Redis:Host");
            var redisBusPort = configurationSection.GetValue<int>("Bus:Redis:Port");
            var redisBusPassword = configurationSection.GetValue<string>("Bus:Redis:Password");
            var redisBusDatabase = configurationSection.GetValue<int>("Bus:Redis:Database");

            Action<RedisOptions> redisConfigure = config =>
            {
                config.DBConfig.Endpoints.Add(new ServerEndPoint(redisCacheHost, redisCachePort));
                config.DBConfig.Database = redisCacheDatabase;
                config.DBConfig.Password = redisCachePassword;

                // 重要配置， 默认是provider的名字
                // .net 5及以上必须配置序列化器名
                config.SerializerName = redisCacheSerializerName;
            };

            Action<HybridCachingOptions> hybridCachingConfigure = config =>
            {
                // 启用日志
                config.EnableLogging = true;
                // 缓存总线的订阅主题
                config.TopicName = hybridCacheTopicName;
                // 本地缓存的名字
                config.LocalCacheProviderName = memoryCacheName;
                // 分布式缓存的名字
                config.DistributedCacheProviderName = redisCacheName;
            };


            Action<RedisBusOptions> redisBusConfigure = config =>
            {
                config.Endpoints.Add(new ServerEndPoint(redisBusHost, redisBusPort));
                config.Database = redisBusDatabase;
                config.Password = redisBusPassword;
            };


            services.AddEasyCaching(options =>
            {
                options.UseInMemory(memoryCacheName);
                options.UseRedis(redisConfigure, redisCacheName);
                //  使用混合缓存
                options.UseHybrid(hybridCachingConfigure);

                // 默认使用TextJson作为序列化器
                //.net 5及以上必须配置序列化器名
                options.WithSystemTextJson(redisCacheSerializerName);


                services.AddEasyCaching(options =>
                {
                    // 使用redis作为缓存总线
                    options.WithRedisBus(redisBusConfigure, redisBusName);
                });
            });
            
            


            return services;
        }

        /// <summary>
        /// 添加混合缓存（内存+Redis+Redis Bus）
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddInMemoryRedisCacheWithRedisBus(this IServiceCollection services, Action<HybridCacheOptions> setupAction)
        {

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            var hybridCacheOptions = new HybridCacheOptions();
            setupAction(hybridCacheOptions);

            if (string.IsNullOrEmpty(hybridCacheOptions.TopicName))
            {
                throw new ArgumentNullException(nameof(hybridCacheOptions.TopicName));
            }

            // InMemory
            if (hybridCacheOptions.InMemory == null)
            {
                throw new ArgumentNullException(nameof(hybridCacheOptions.InMemory));
            }

            if (string.IsNullOrEmpty(hybridCacheOptions.InMemory.CacheName))
            {
                throw new ArgumentNullException(nameof(hybridCacheOptions.InMemory.CacheName));
            }

            // Redis
            if (hybridCacheOptions.Redis == null)
            {
                throw new ArgumentNullException(nameof(hybridCacheOptions.Redis));
            }

            if (string.IsNullOrEmpty(hybridCacheOptions.Redis.CacheName))
            {
                throw new ArgumentNullException(nameof(hybridCacheOptions.Redis.CacheName));
            }

            if (string.IsNullOrEmpty(hybridCacheOptions.Redis.Host))
            {
                throw new ArgumentNullException(nameof(hybridCacheOptions.Redis.Host));
            }

            if (hybridCacheOptions.Redis.Port <= 0)
            {
                throw new ArgumentNullException(nameof(hybridCacheOptions.Redis.Port));
            }

            if (string.IsNullOrEmpty(hybridCacheOptions.Redis.SerializerName))
            {
                throw new ArgumentNullException(nameof(hybridCacheOptions.Redis.SerializerName));
            }

            if (hybridCacheOptions.Redis.Database < 0)
            {
                throw new ArgumentNullException(nameof(hybridCacheOptions.Redis.Database));
            }

            // Bus
            if (hybridCacheOptions.Bus == null)
            {
                throw new ArgumentNullException(nameof(hybridCacheOptions.Bus));
            }
            if (hybridCacheOptions.Bus.Redis == null)
            {
                throw new ArgumentNullException(nameof(hybridCacheOptions.Bus.Redis));
            }

            if (string.IsNullOrEmpty(hybridCacheOptions.Bus.Redis.Name))
            {
                throw new ArgumentNullException(nameof(hybridCacheOptions.Bus.Redis.Name));
            }

            if (string.IsNullOrEmpty(hybridCacheOptions.Bus.Redis.Host))
            {
                throw new ArgumentNullException(nameof(hybridCacheOptions.Bus.Redis.Host));
            }

            if (hybridCacheOptions.Bus.Redis.Port <= 0)
            {
                throw new ArgumentNullException(nameof(hybridCacheOptions.Bus.Redis.Port));
            }


            if (hybridCacheOptions.Bus.Redis.Database < 0)
            {
                throw new ArgumentNullException(nameof(hybridCacheOptions.Bus.Redis.Database));
            }

            services.AddSingleton(hybridCacheOptions);

            var memoryCacheName = hybridCacheOptions.InMemory.CacheName;

            var hybridCacheTopicName = hybridCacheOptions.TopicName;
            var redisCacheHost = hybridCacheOptions.Redis.Host;
            var redisCachePort = hybridCacheOptions.Redis.Port;
            var redisCachePassword = hybridCacheOptions.Redis.Password;
            var redisCacheSerializerName = hybridCacheOptions.Redis.SerializerName;
            var redisCacheName = hybridCacheOptions.Redis.CacheName;
            var redisCacheDatabase = hybridCacheOptions.Redis.Database;
            
            var redisBusName = hybridCacheOptions.Bus.Redis.Name;
            var redisBusHost = hybridCacheOptions.Bus.Redis.Host;
            var redisBusPort = hybridCacheOptions.Bus.Redis.Port;
            var redisBusPassword = hybridCacheOptions.Bus.Redis.Password;
            var redisBusDatabase = hybridCacheOptions.Bus.Redis.Database;

            Action<RedisOptions> redisConfigure = config =>
            {
                config.DBConfig.Endpoints.Add(new ServerEndPoint(redisCacheHost, redisCachePort));
                config.DBConfig.Database = redisCacheDatabase;
                config.DBConfig.Password = redisCachePassword;

                // 重要配置， 默认是provider的名字
                // .net 5及以上必须配置序列化器名
                config.SerializerName = redisCacheSerializerName;
            };

            Action<HybridCachingOptions> hybridCachingConfigure = config =>
            {
                // 启用日志
                config.EnableLogging = true;
                // 缓存总线的订阅主题
                config.TopicName = hybridCacheTopicName;
                // 本地缓存的名字
                config.LocalCacheProviderName = memoryCacheName;
                // 分布式缓存的名字
                config.DistributedCacheProviderName = redisCacheName;
            };


            Action<RedisBusOptions> redisBusConfigure = config =>
            {
                config.Endpoints.Add(new ServerEndPoint(redisBusHost, redisBusPort));
                config.Database = redisBusDatabase;
                config.Password = redisBusPassword;
            };


            services.AddEasyCaching(options =>
            {
                options.UseInMemory(memoryCacheName);
                options.UseRedis(redisConfigure, redisCacheName);
                //  使用混合缓存
                options.UseHybrid(hybridCachingConfigure);

                // 默认使用TextJson作为序列化器
                //.net 5及以上必须配置序列化器名
                options.WithSystemTextJson(redisCacheSerializerName);


                services.AddEasyCaching(options =>
                {
                    // 使用redis作为缓存总线
                    options.WithRedisBus(redisBusConfigure, redisBusName);
                });
            });
            
            


            return services;
        }

    }
}
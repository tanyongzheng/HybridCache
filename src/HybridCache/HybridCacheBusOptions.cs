namespace HybridCache
{
    public class HybridCacheBusOptions
    {
        public BusWithRedisOptions Redis { get; set; }
        public BusWithRabbitMQOptions RabbitMQ { get; set; }
    }
}
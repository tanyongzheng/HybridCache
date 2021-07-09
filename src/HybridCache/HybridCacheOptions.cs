namespace HybridCache
{
    public class HybridCacheOptions
    {
        public InMemoryCacheOptions InMemory { get; set; }

        public RedisCacheOptions Redis { get; set; }

        public string TopicName { get; set; }

        public HybridCacheBusOptions Bus { get; set; }
    }
}
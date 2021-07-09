namespace HybridCache
{
    /// <summary>
    /// Redis缓存选项
    /// </summary>
    public class RedisCacheOptions
    {
        /// <summary>
        /// 缓存名
        /// </summary>
        public string CacheName { get; set; }

        /// <summary>
        /// 主机
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Redis数据库索引
        /// </summary>
        public int Database { get; set; }

        /// <summary>
        /// .net 5及以上必须配置序列化器名
        /// 缓存配置和总线配置的序列化名需一致
        /// </summary>
        public string SerializerName { get; set; }
    }
}
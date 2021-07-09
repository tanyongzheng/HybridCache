using EasyCaching.Core;

namespace HybridCache
{
    public class BusWithRedisOptions
    {

        /// <summary>
        /// Redis名，默认DefaultRedis
        /// </summary>
        public string Name { get; set; } = EasyCachingConstValue.DefaultRedisName;

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
    }
}
using System;

namespace HybridCache.Demo.Domain
{
    [Serializable]
    public class UserCacheItem
    {
        public const string HybridCacheName = "HybridCacheItemKey:" + nameof(UserCacheItem);

        public string UserName { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PhoneNumber { get; set; }
    }
}
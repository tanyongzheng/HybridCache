using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCaching.Core;

namespace HybridCache.Demo.Domain
{
    public class UserManager
    {
        private readonly IHybridCachingProvider _hybridCachingProvider;

        public UserManager(IHybridCachingProvider hybridCachingProvider)
        {
            _hybridCachingProvider = hybridCachingProvider;
        }

        public async Task<List<UserCacheItem>> GetListFromCacheAsync()
        {
            if (await _hybridCachingProvider.ExistsAsync(UserCacheItem.HybridCacheName))
            {
                var hybridCachingResult =
                    await _hybridCachingProvider.GetAsync<List<UserCacheItem>>(UserCacheItem.HybridCacheName);
                if (hybridCachingResult.HasValue)
                {
                    return hybridCachingResult.Value;
                }
            }
            var list = GetList();
            var cacheItem = list;
            await _hybridCachingProvider.SetAsync(UserCacheItem.HybridCacheName, cacheItem, TimeSpan.FromMinutes(10));
            return cacheItem;
        }
        public async Task<bool> RemoveCacheAsync()
        {
            if (await _hybridCachingProvider.ExistsAsync(UserCacheItem.HybridCacheName))
            {
                await _hybridCachingProvider.RemoveAsync(UserCacheItem.HybridCacheName);
            }

            return true;
        }

        private List<UserCacheItem> GetList()
        {
            var list = new List<UserCacheItem>()
            {
                new UserCacheItem()
                {
                    UserName = "李白"
                },
                new UserCacheItem()
                {
                    UserName = "杜甫"
                },
                new UserCacheItem()
                {
                    UserName = "白居易"
                },
            };
            return list;
        }
    }
}
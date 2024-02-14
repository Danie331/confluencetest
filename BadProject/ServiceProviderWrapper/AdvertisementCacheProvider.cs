using BadProject.DomainType;
using BadProject.ServiceProviderWrapper.Contract;
using System;
using System.Runtime.Caching;

namespace BadProject.ServiceProviderWrapper
{
    public class AdvertisementCacheProvider : IAdvertisementCacheProvider
    {
        private readonly MemoryCache _cache;
        public AdvertisementCacheProvider(MemoryCache cacheProvider)
        {
            _cache = cacheProvider;
        }

        public Advertisement GetAdvertisement(string id)
        {
            return (Advertisement)_cache.Get($"AdvKey_{id}");
        }

        public void Set(Advertisement adv, string id)
        {
            _cache.Set($"AdvKey_{id}", adv, DateTimeOffset.Now.AddMinutes(5));
        }
    }
}

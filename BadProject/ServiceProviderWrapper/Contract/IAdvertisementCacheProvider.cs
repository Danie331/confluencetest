using BadProject.DomainType;

namespace BadProject.ServiceProviderWrapper.Contract
{
    public interface IAdvertisementCacheProvider 
    {
        Advertisement GetAdvertisement(string id);
        void Set(Advertisement adv, string id);
    }
}

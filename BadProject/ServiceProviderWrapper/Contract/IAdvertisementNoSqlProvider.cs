using BadProject.DomainType;

namespace BadProject.ServiceProviderWrapper.Contract
{
    public interface IAdvertisementNoSqlProvider
    {
        Advertisement GetAdvertisement(string id);
    }
}

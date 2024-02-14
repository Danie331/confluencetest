
using BadProject.DomainType;

namespace BadProject.ServiceProviderWrapper.Contract
{
    public interface IAdvertisementSqlProvider
    {
        Advertisement GetAdvertisement(string id);
    }
}

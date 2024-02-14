using BadProject.ServiceProviderWrapper.Contract;
using ThirdParty;
using Domain = BadProject.DomainType;

namespace BadProject.ServiceProviderWrapper
{
    public class AdvertisementNoSqlProvider : IAdvertisementNoSqlProvider
    {
        public Domain.Advertisement GetAdvertisement(string id)
        {
            var advDto = new NoSqlAdvProvider().GetAdv(id);
            // Simple naieve mapping
            return new Domain.Advertisement
            {
                Description = advDto.Description,
                Name = advDto.Name,
                WebId = advDto.WebId,
            };
        }
    }
}

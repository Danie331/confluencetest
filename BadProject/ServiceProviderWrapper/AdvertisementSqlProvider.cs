using BadProject.ServiceProviderWrapper.Contract;
using ThirdParty;
using Domain = BadProject.DomainType;

namespace BadProject.ServiceProviderWrapper
{
    public class AdvertisementSqlProvider : IAdvertisementSqlProvider
    {
        public Domain.Advertisement GetAdvertisement(string id)
        {
            var advDto = SQLAdvProvider.GetAdv(id);
            return new Domain.Advertisement
            {
                Description = advDto.Description,
                Name = advDto.Name,
                WebId = advDto.WebId,
            };
        }
    }
}

using Autofac;
using BadProject.Service;
using BadProject.Service.Contract;
using BadProject.ServiceProviderWrapper;
using BadProject.ServiceProviderWrapper.Contract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace Tests
{
    [TestClass]
    public class AdvertismentTests
    {
        private IContainer _container;
        [TestMethod]
        public void Test_ContainerResolvesTypes()
        {
            var advertisementService = _container.Resolve(typeof(IAdvertisementService));
            Assert.IsNotNull(advertisementService);

            var advertisementCacheProvider = _container.Resolve(typeof(IAdvertisementCacheProvider));
            Assert.IsNotNull(advertisementCacheProvider);

            var advertisementNoSqlProvider = _container.Resolve(typeof(IAdvertisementNoSqlProvider));
            Assert.IsNotNull(advertisementNoSqlProvider);

            var advertisementSqlProvider = _container.Resolve(typeof(IAdvertisementSqlProvider));
            Assert.IsNotNull(advertisementSqlProvider);

            MemoryCache memoryCache = _container.Resolve<MemoryCache>();
            Assert.IsNotNull(memoryCache);
        
            Queue<DateTime> queue = _container.Resolve<Queue<DateTime>>();
            Assert.IsNotNull(queue);
        }

        [TestMethod]
        public void Test_AdvertismentWorkflow()
        {
            var advService = _container.Resolve<IAdvertisementService>();
            var cache = _container.Resolve<MemoryCache>();

            // Should pull from 3rd party and add to cache
            var adv1 = advService.GetAdvertisement("adv1");

            Assert.IsNotNull(adv1);
            Assert.IsTrue(cache.GetCount() == 1);

            // Should retrieve from cache
            var adv1FromCache = advService.GetAdvertisement("adv1");

            Assert.IsNotNull(adv1);
            Assert.IsTrue(cache.GetCount() == 1); // Should still be 1

            var adv2 = advService.GetAdvertisement("adv2");

            Assert.IsNotNull(adv2);
            Assert.IsTrue(cache.GetCount() == 2); // Should increase
        }

        [TestInitialize]
        public void Init()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(typeof(BadProject.Service.AdvertisementService).Assembly);
            builder.RegisterInstance(new MemoryCache("xyz")).SingleInstance();
            builder.RegisterType<Queue<DateTime>>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType(typeof(AdvertisementCacheProvider)).AsImplementedInterfaces();
            builder.RegisterType(typeof(AdvertisementNoSqlProvider)).AsImplementedInterfaces();
            builder.RegisterType(typeof(AdvertisementSqlProvider)).AsImplementedInterfaces();
            builder.RegisterType(typeof(AdvertisementService)).AsImplementedInterfaces();

            _container = builder.Build();
        }
    }
}

using BadProject.DomainType;
using BadProject.Service.Contract;
using BadProject.ServiceProviderWrapper.Contract;
using System;
using System.Collections.Generic;
using System.Configuration;
//using ThirdParty; -- domain-level service should not directly interface with third party.

namespace BadProject.Service
{
    /// <summary>
    /// Service definition
    /// Injected must be injected as instance-per-dependency / transient
    /// </summary>
    public class AdvertisementService : IAdvertisementService
    {
        /*** 
         * 
         * Assumption: because these are private members the intention is not to use them outside of this
         * service, therefore in a DI environment we can inject these as needed as long as this service is 
         * scoped to provide a unique instance-per-dependency for thread safety.
         * 
        private static MemoryCache cache = new MemoryCache("something");
        private static Queue<DateTime> errors = new Queue<DateTime>();
        ***/
        private readonly Queue<DateTime> _errors;
        private readonly IAdvertisementCacheProvider _cacheProvider;
        private readonly IAdvertisementNoSqlProvider _noSqlProvider;
        private readonly IAdvertisementSqlProvider _sqlProvider;
        public AdvertisementService(Queue<DateTime> errorProvider,
                                    IAdvertisementCacheProvider cacheProvider,
                                    IAdvertisementNoSqlProvider noSqlProvider,
                                    IAdvertisementSqlProvider sqlProvider)
        {
            _errors = errorProvider;
            _cacheProvider = cacheProvider;
            _noSqlProvider = noSqlProvider;
            _sqlProvider = sqlProvider;
        }

        /***
         * 
         * No need to lock if using DI providing a unique instance-per-dependency, and given assumption above.
         * 
        private Object lockObj = new Object();
        ***/
        // **************************************************************************************************
        // Loads Advertisement information by id
        // from cache or if not possible uses the "mainProvider" or if not possible uses the "backupProvider"
        // **************************************************************************************************
        // Detailed Logic:
        // 
        // 1. Tries to use cache (and retuns the data or goes to STEP2)
        //
        // 2. If the cache is empty it uses the NoSqlDataProvider (mainProvider), 
        //    in case of an error it retries it as many times as needed based on AppSettings
        //    (returns the data if possible or goes to STEP3)
        //
        // 3. If it can't retrive the data or the ErrorCount in the last hour is more than 10, 
        //    it uses the SqlDataProvider (backupProvider)
        public Advertisement GetAdvertisement(string id)
        {
            // Use Cache if available
            Advertisement adv = _cacheProvider.GetAdvertisement(id);

            // Count HTTP error timestamps in the last hour
            while (_errors.Count > 20) _errors.Dequeue();
            int errorCount = 0;
            foreach (var dat in _errors)
            {
                if (dat > DateTime.Now.AddHours(-1))
                {
                    errorCount++;
                }
            }

            // If Cache is empty and ErrorCount<10 then use HTTP provider
            if ((adv == null) && (errorCount < 10))
            {
                int retry = 0;
                do
                {
                    retry++;
                    try
                    {
                        adv = _noSqlProvider.GetAdvertisement(id);
                    }
                    catch
                    {
                        // Thread.Sleep(1000); not sure why this is needed?
                        _errors.Enqueue(DateTime.Now); // Store HTTP error timestamp              
                    }
                } while ((adv == null) && (retry < int.Parse(ConfigurationManager.AppSettings["RetryCount"])));


                if (adv != null)
                {
                    _cacheProvider.Set(adv, id);
                }
            }

            // if needed try to use Backup provider
            if (adv == null)
            {
                adv = _sqlProvider.GetAdvertisement(id);

                if (adv != null)
                {
                    _cacheProvider.Set(adv, id);
                }
            }

            return adv;
        }
    }
}
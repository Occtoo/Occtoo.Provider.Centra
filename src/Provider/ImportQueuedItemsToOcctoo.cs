using AutoMapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Occtoo.Onboarding.Sdk.Models;
using Occtoo.Provider.Centra.Models;
using Occtoo.Provider.Centra.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Occtoo.Provider.Centra
{
    public class ImportQueuedItemsToOcctoo
    {
        private readonly ICentraService _centraService;
        private readonly IMapper _mapper;
        private readonly IOcctooExporter _occtooExporter;
        private readonly AppSettings _appSettings;
        private readonly ITokenService _tokenService;

        public ImportQueuedItemsToOcctoo(ICentraService centraService, IMapper mapper, IOcctooExporter occtooExporter, AppSettings appSettings, ITokenService tokenService)
        {
            _centraService = centraService;
            _mapper = mapper;
            _occtooExporter = occtooExporter;
            _appSettings = appSettings;
            _tokenService = tokenService;
        }

        /// <summary>
        /// This is where all your business logic should go.
        /// This will trigger whenver you add something to the que.
        /// </summary>
        /// <param name="myQueueItem">This is the item that was passed in the que</param>
        /// <param name="log"></param>
        [FunctionName(nameof(ImportQueuedItemsToOcctoo))]
        public async Task Run([QueueTrigger("%myqueue-items%", Connection = "AzureWebJobsStorage")] string productSku, ILogger log)
        {
            var products = await _centraService.GetProductsByProductNumber(productSku);
            var productsForOnboarding = _mapper.Map<ProductOnboardingModel>(products);

            // remove products not fulfilling the business logic
            productsForOnboarding.Variants = productsForOnboarding.Variants
                .Where(x => !string.IsNullOrEmpty(x.id)
                    && !string.IsNullOrEmpty(x.ProductSku)
                    && !string.IsNullOrEmpty(x.mediaUrls)
                ).ToList();

            if (productsForOnboarding.Variants.Any())
            {
                var token = await _tokenService.GetProviderToken(nameof(ImportQueuedItemsToOcctoo), Environment.GetEnvironmentVariable("DataProviderId"), Environment.GetEnvironmentVariable("DataProviderSecret"));

                var productSources = await _occtooExporter.GetProductDynamicEntitiesAsync(productsForOnboarding);
                var variantSources = productsForOnboarding.Variants.Select(x => _occtooExporter.GetVariantDynamicEntitiesAsync(x).Result).ToList();
                var stockSources = productsForOnboarding.Variants.SelectMany(x => x.stocks.Select(d => _occtooExporter.GetStockDynamicEntitiesAsync(d).Result)).ToList();
                var priceSources = productsForOnboarding.Variants.SelectMany(x => x.prices.Select(d => _occtooExporter.GetPriceDynamicEntitiesAsync(d).Result)).ToList();

                var response = await _occtooExporter.StartEntityImportAsync(_appSettings.ProductSource, new List<DynamicEntity> { productSources }, token);
                var response2 = await _occtooExporter.StartEntityImportAsync(_appSettings.VariantSource, variantSources, token);
                var response3 = await _occtooExporter.StartEntityImportAsync(_appSettings.StockSource, stockSources, token);
                var response4 = await _occtooExporter.StartEntityImportAsync(_appSettings.PriceSource, priceSources, token);
            }

            log.LogInformation($"C# Queue trigger function processed: {productSku}");
        }
    }
}

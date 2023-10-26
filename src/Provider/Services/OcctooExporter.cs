using Occtoo.Onboarding.Sdk;
using Occtoo.Onboarding.Sdk.Models;
using Occtoo.Provider.Centra.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Occtoo.Provider.Centra.Services
{
    public interface IOcctooExporter
    {
        Task<DynamicEntity> GetProductDynamicEntitiesAsync(ProductOnboardingModel items);
        Task<DynamicEntity> GetVariantDynamicEntitiesAsync(VariantOnboardingModel items);
        Task<DynamicEntity> GetStockDynamicEntitiesAsync(StockOnboardingModel items);
        Task<DynamicEntity> GetPriceDynamicEntitiesAsync(PriceOnboardingModel items);
        Task<StartImportResponse> StartEntityImportAsync(string dataSource, IReadOnlyList<DynamicEntity> entities, string token, Guid? correlationId = null, CancellationToken? cancellationToken = null);
    }
    public class OcctooExporter : IOcctooExporter
    {
        private IOnboardingServiceClient _onboardingServliceClient;
        public OcctooExporter(IOnboardingServiceClient onboardingServiceClient)
        {
            _onboardingServliceClient = onboardingServiceClient;
        }

        public async Task<StartImportResponse> StartEntityImportAsync(string dataSource, IReadOnlyList<DynamicEntity> entities, string token, Guid? correlationId = null, CancellationToken? cancellationToken = null)
        {
            return await _onboardingServliceClient.StartEntityImportAsync(dataSource, entities, token, correlationId, cancellationToken);
        }

        public async Task<DynamicEntity> GetProductDynamicEntitiesAsync(ProductOnboardingModel items)
        {
            return await Task.Run(() =>
            {
                DynamicEntity dynamicEntity = new();
                dynamicEntity.Key = items.Id;
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.Id), Value = items.Id });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.variantIds), Value = items.variantIds });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.brandName), Value = items.brandName });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.weight), Value = items.weight });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.collectionName), Value = items.collectionName });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.Name), Value = items.Name });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.weightUnit), Value = items.weightUnit });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.countryOfOrigin), Value = items.countryOfOrigin });
                return dynamicEntity;
            });
        }
        public async Task<DynamicEntity> GetVariantDynamicEntitiesAsync(VariantOnboardingModel items)
        {
            return await Task.Run(() =>
            {
                DynamicEntity dynamicEntity = new();
                dynamicEntity.Key = items.id;
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.id), Value = items.id });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.ProductSku), Value = items.ProductSku });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.weightUnit), Value = items.weightUnit });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.countryOfOrigin), Value = items.countryOfOrigin });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.weight), Value = items.weight });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.collectionName), Value = items.collectionName });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.brandName), Value = items.brandName });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.material), Value = items.material });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.gender), Value = items.gender });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.folderName), Value = items.folderName });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.category), Value = items.category });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.color), Value = items.color });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.description), Value = items.description });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.fit), Value = items.fit });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.fabric), Value = items.fabric });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.mediaUrls), Value = items.mediaUrls });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.thumbnail), Value = items.thumbnail });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.Sizes), Value = items.Sizes });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.Name), Value = items.Name });

                return dynamicEntity;
            });
        }

        public async Task<DynamicEntity> GetStockDynamicEntitiesAsync(StockOnboardingModel items)
        {
            return await Task.Run(() =>
            {
                DynamicEntity dynamicEntity = new();
                dynamicEntity.Key = items.id;
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.variantId), Value = items.variantId });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.id), Value = items.id });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.warehouseId), Value = items.warehouseId });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.GTIN), Value = items.GTIN });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.sku), Value = items.sku });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.availableQuantity), Value = items.availableQuantity });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.physicalQuantity), Value = items.physicalQuantity });

                return dynamicEntity;
            });
        }
        public async Task<DynamicEntity> GetPriceDynamicEntitiesAsync(PriceOnboardingModel items)
        {
            return await Task.Run(() =>
            {
                DynamicEntity dynamicEntity = new();
                dynamicEntity.Key = items.id;
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.id), Value = items.id });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.variantId), Value = items.variantId });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.productSku), Value = items.productSku });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.formattedValue), Value = items.formattedValue });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.value), Value = items.value });
                dynamicEntity.Properties.Add(new DynamicProperty { Id = nameof(items.currency), Value = items.currency });

                return dynamicEntity;
            });
        }
    }
}

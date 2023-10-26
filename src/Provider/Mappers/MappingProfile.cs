using AutoMapper;
using Occtoo.Provider.Centra.Models;
using System.Collections.Generic;
using System.Linq;

namespace Occtoo.Provider.Centra.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<List<ProductGraphQlResponseModel>, ProductOnboardingModel>()
                .ForMember(dest => dest.brandName, act => act.MapFrom(src => src.FirstOrDefault().brand.name))
                .ForMember(dest => dest.countryOfOrigin, act => act.MapFrom(src => src.FirstOrDefault().countryOfOrigin.name))
                .ForMember(dest => dest.Id, act => act.MapFrom(src => src.FirstOrDefault().productNumber))
                .ForMember(dest => dest.Name, act => act.MapFrom(src => src.FirstOrDefault().name))
                .ForMember(dest => dest.Variants, act => act.MapFrom(src => src))
                .ForMember(dest => dest.collectionName, act => act.MapFrom(src => src.FirstOrDefault().collection.name))
                .ForMember(dest => dest.weight, act => act.MapFrom(src => src.FirstOrDefault().weight.value.ToString()))
                .ForMember(dest => dest.weightUnit, act => act.MapFrom(src => src.FirstOrDefault().weight.unit));


            CreateMap<ProductGraphQlResponseModel, VariantOnboardingModel>()
                .ForMember(dest => dest.ProductSku, act => act.MapFrom(src => src.productNumber))
                .ForMember(dest => dest.folderName, act => act.MapFrom(src => src.folder))
                .ForMember(dest => dest.category, act => act.MapFrom(src => string.IsNullOrEmpty(src.collection.name) ? "No category" : src.collection.name))
                .ForMember(dest => dest.material, act => act.MapFrom(src => DetermineAttribute(src, "Google Merchant g:material")))
                .ForMember(dest => dest.color, act => act.MapFrom(src => string.IsNullOrEmpty(src.variants.FirstOrDefault().name) ? DetermineAttribute(src, "Google Merchant g:color") : src.variants.FirstOrDefault().name))
                .ForMember(dest => dest.Name, act => act.MapFrom(src => src.variants.FirstOrDefault().name))
                .ForMember(dest => dest.mediaUrls, act => act.MapFrom(src => string.Join("|", src.media.Select(x => x.source.url))))
                .ForMember(dest => dest.thumbnail, act => act.MapFrom(src => src.media.FirstOrDefault().source.url))
                .ForMember(dest => dest.id, act => act.MapFrom(src => GetVariantId(src)))
                .ForMember(dest => dest.Sizes, act => act.MapFrom(src => GetSizesFromFields(src)))
                .ForMember(dest => dest.stocks, act => act.MapFrom(src => GetStocksFromModel(src)))
                .ForMember(dest => dest.prices, act => act.MapFrom(src => GetPricesFromModel(src)))
                .ForMember(dest => dest.gender, act => act.MapFrom(src => DetermineGenderFromName(src.collection.name)))
                .ForMember(dest => dest.brandName, act => act.MapFrom(src => src.brand.name))
                .ForMember(dest => dest.countryOfOrigin, act => act.MapFrom(src => src.countryOfOrigin.name))
                .ForMember(dest => dest.collectionName, act => act.MapFrom(src => src.collection.name))
                .ForMember(dest => dest.weight, act => act.MapFrom(src => src.weight.value.ToString()))
                .ForMember(dest => dest.weightUnit, act => act.MapFrom(src => src.weight.unit));
        }

        static List<StockOnboardingModel> GetStocksFromModel(ProductGraphQlResponseModel model)
        {
            List<StockOnboardingModel> stocks = new List<StockOnboardingModel>();
            var variantStocks = model.variants.FirstOrDefault().stock;
            foreach (var variantStock in variantStocks)
            {
                if (!string.IsNullOrEmpty(variantStock.productSize.GTIN)
                    && !string.IsNullOrEmpty(variantStock.warehouse.name) && variantStock.warehouse.id != 0)
                {
                    StockOnboardingModel tempStock = new StockOnboardingModel
                    {
                        availableQuantity = variantStock.availableNowQuantity.ToString(),
                        id = variantStock.productSize.GTIN + "-" + variantStock.warehouse.id.ToString(),
                        physicalQuantity = variantStock.physicalQuantity.ToString(),
                        warehouseId = variantStock.warehouse.id.ToString(),
                        sku = model.productNumber,
                        variantId = GetVariantId(model),
                        warehouse = variantStock.warehouse.name,
                        GTIN = variantStock.productSize.GTIN
                    };
                    stocks.Add(tempStock);
                }
            }
            return stocks;
        }
        static List<PriceOnboardingModel> GetPricesFromModel(ProductGraphQlResponseModel model)
        {
            List<PriceOnboardingModel> prices = new List<PriceOnboardingModel>();
            foreach (var price in model.variants.FirstOrDefault().prices)
            {
                if (price.price != null)
                {
                    PriceOnboardingModel tempPrice = new PriceOnboardingModel
                    {
                        productSku = model.productNumber,
                        currency = price.price.currency.name,
                        formattedValue = price.price.formattedValue,
                        id = price.id.ToString(),
                        variantId = GetVariantId(model),
                        value = price.price.valueInCents.ToString()
                    };
                    prices.Add(tempPrice);
                }
            }
            return prices;
        }
        static string GetSizesFromFields(ProductGraphQlResponseModel model)
        {
            var material = model.displays.FirstOrDefault();
            if (material != null)
            {
                var productVariants = material.productVariants.FirstOrDefault();
                if (productVariants != null && productVariants.productSizes != null && productVariants.productSizes.Any())
                {
                    var productSizes = productVariants.productSizes.Select(x => x.description).ToList();

                    return string.Join("|", productSizes);
                }
            }
            return "No size";
        }

        static string DetermineAttribute(ProductGraphQlResponseModel model, string attributeDescription)
        {
            var variants = model.variants;
            if (variants != null)
            {
                var attributes = variants.FirstOrDefault().attributes;
                if (attributes != null)
                {
                    var material = attributes.FirstOrDefault(x => x.description == attributeDescription);
                    if (material != null)
                    {
                        var element = material.elements.FirstOrDefault();
                        if (element != null)
                            return element.value;
                    }
                }
            }
            return "No color";
        }
        static string DetermineGenderFromName(string name)
        {
            name = name.ToLower();
            List<string> genders = new List<string> { "male", "female", "kids", "other" };
            if (name.Contains("men"))
            {
                return genders[0];
            }

            if (name.Contains("women"))
            {
                return genders[1];
            }

            if (name.Contains("kids"))
            {
                return genders[2];
            }
            return genders[3];

        }
        static string GetVariantId(ProductGraphQlResponseModel model)
        {
            var material = model.displays.FirstOrDefault();
            if (material != null)
            {
                if (material.displayItems != null && material.displayItems.Any())
                {
                    var variantId = material.displayItems.FirstOrDefault().id;

                    return model.productNumber + "-" + variantId.ToString();
                }
            }
            return "";
        }
    }
}

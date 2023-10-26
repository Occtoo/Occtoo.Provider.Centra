using System.Collections.Generic;

namespace Occtoo.Provider.Centra.Models
{
    public class ProductGraphQlResponseModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public Collection collection { get; set; }
        public Weight weight { get; set; }
        public string harmonizedCommodityCodeDescription { get; set; }
        public string harmonizedCommodityCode { get; set; }
        public CountryOfOrigin countryOfOrigin { get; set; }
        public Folder folder { get; set; }
        public List<Display> displays { get; set; }
        public Brand brand { get; set; }
        public string productNumber { get; set; }
        public List<Medium> media { get; set; }
        public List<Variant> variants { get; set; }
        public List<Price> prices { get; set; }
    }

    public class Attribute
    {
        public List<Element> elements { get; set; }
        public string description { get; set; }
        public string objectType { get; set; }
        public Type type { get; set; }
    }

    public class Brand
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Collection
    {
        public int id { get; set; }
        public string name { get; set; }
        public string uri { get; set; }
        public string status { get; set; }
    }

    public class CountryOfOrigin
    {
        public string name { get; set; }
        public List<Translation> translations { get; set; }
    }

    public class Currency
    {
        public string name { get; set; }
        public string code { get; set; }
    }

    public class Data
    {
        public List<ProductGraphQlResponseModel> products { get; set; }
    }

    public class Display
    {
        public List<Market> markets { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public string status { get; set; }
        public string metaTitle { get; set; }
        public string metaKeywords { get; set; }
        public string metaDescription { get; set; }
        public string description { get; set; }
        public List<ProductVariant> productVariants { get; set; }
        public List<DisplayItem> displayItems { get; set; }
    }

    public class ProductVariant
    {
        public List<ProductSize> productSizes { get; set; }
    }

    public class DisplayItem
    {
        public int id { get; set; }
        public Display display { get; set; }
    }

    public class Element
    {
        public string key { get; set; }
        public string description { get; set; }
        public string kind { get; set; }
        public string value { get; set; }
    }

    public class Extensions
    {
        public int complexity { get; set; }
        public List<string> permissionsUsed { get; set; }
        public string appVersion { get; set; }
    }

    public class Field
    {
        public string value { get; set; }
    }

    public class Folder
    {
        public string name { get; set; }
        public List<object> childFolders { get; set; }
    }

    public class Language
    {
        public string name { get; set; }
        public string countryCode { get; set; }
    }

    public class Market
    {
        public string name { get; set; }
    }

    public class Medium
    {
        public Source source { get; set; }
    }

    public class Price
    {
        public int id { get; set; }
        public string recommendedRetailPrice { get; set; }
        public Price2 price { get; set; }
    }

    public class Price2
    {
        public double value { get; set; }
        public Currency currency { get; set; }
        public string formattedValue { get; set; }
        public int valueInCents { get; set; }
    }

    public class ProductSize
    {
        public int id { get; set; }
        public string GTIN { get; set; }
        public string sizeNumber { get; set; }
        public string SKU { get; set; }
        public string description { get; set; }
    }

    public class Root
    {
        public Data data { get; set; }
        public Extensions extensions { get; set; }
    }

    public class Source
    {
        public string url { get; set; }
    }

    public class Stock
    {
        public int physicalQuantity { get; set; }
        public int availableNowQuantity { get; set; }
        public ProductSize productSize { get; set; }
        public Warehouse warehouse { get; set; }
    }

    public class Supplier
    {
        public string name { get; set; }
    }

    public class Supply
    {
        public Supplier supplier { get; set; }
    }

    public class Translation
    {
        public Language language { get; set; }
        public List<Field> fields { get; set; }
    }

    public class Type
    {
        public string name { get; set; }
    }

    public class Variant
    {
        public object internalName { get; set; }
        public List<Supply> supply { get; set; }
        public List<Attribute> attributes { get; set; }
        public string name { get; set; }
        public object variantNumber { get; set; }
        public List<Translation> translations { get; set; }
        public List<Stock> stock { get; set; }
        public List<Price> prices { get; set; }
    }

    public class Warehouse
    {
        public int id { get; set; }
        public string name { get; set; }
        public string status { get; set; }
    }

    public class Weight
    {
        public double value { get; set; }
        public string unit { get; set; }
        public string formattedValue { get; set; }
    }
}

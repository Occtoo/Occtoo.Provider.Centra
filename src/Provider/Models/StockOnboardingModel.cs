namespace Occtoo.Provider.Centra.Models
{
    public class StockOnboardingModel
    {
        public string variantId { get; set; }
        public string id { get; set; }
        public string warehouse { get; set; }
        public string warehouseId { get; set; }
        public string GTIN { get; set; }
        public string sku { get; set; }
        public string physicalQuantity { get; set; }
        public string availableQuantity { get; set; }
    }
}

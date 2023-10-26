namespace Occtoo.Provider.Centra.Models
{
    public class PriceOnboardingModel
    {
        public string id { get; set; }
        public string productSku { get; set; }
        public string variantId { get; set; }
        public string currency { get; set; }
        public string value { get; set; }
        public string formattedValue { get; set; }
    }
}

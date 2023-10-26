namespace Occtoo.Provider.Centra.Models
{
    public class AppSettings
    {
        public string AzureWebJobsStorage { get; set; }
        public string ProductSource { get; set; }
        public string VariantSource { get; set; }
        public string StockSource { get; set; }
        public string PriceSource { get; set; }

        public string DataProviderId { get; set; }
        public string DataProviderSecret { get; set; }
    }
}

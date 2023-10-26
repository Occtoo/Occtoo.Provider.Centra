using System.Collections.Generic;

namespace Occtoo.Provider.Centra.Models
{
    public class VariantOnboardingModel
    {
        public string id { get; set; }
        public List<StockOnboardingModel> stocks { get; set; }
        public List<PriceOnboardingModel> prices { get; set; }
        public string color { get; set; }
        public string category { get; set; }
        public string Name { get; set; }

        public string ProductSku { get; set; }
        public string Sizes { get; set; }
        public string folderName { get; set; }

        public string foldersName { get; set; }

        public string description { get; set; }

        public string fabric { get; set; }
        public string fit { get; set; }
        public string gender { get; set; }
        public string material { get; set; }
        public string mediaUrls { get; set; }
        public string thumbnail { get; set; }

        public string brandName { get; set; }
        public string collectionName { get; set; }
        public string weight { get; set; }
        public string weightUnit { get; set; }
        public string countryOfOrigin { get; set; }

    }
}

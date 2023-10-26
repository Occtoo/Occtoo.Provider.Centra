using System.Collections.Generic;
using System.Linq;

namespace Occtoo.Provider.Centra.Models
{
    public class ProductOnboardingModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string countryOfOrigin { get; set; }
        public List<VariantOnboardingModel> Variants { get; set; }
        public string brandName { get; set; }
        public string collectionName { get; set; }
        public string variantIds { get { return string.Join("|", Variants.Select(x => x.id)); } }
        public string weight { get; set; }
        public string weightUnit { get; set; }
    }
}

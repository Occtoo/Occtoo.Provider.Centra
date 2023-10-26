using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Occtoo.Provider.Centra.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Occtoo.Provider.Centra
{
    public class FillQueueFromCentra
    {
        private readonly ICentraService _centraService;

        public FillQueueFromCentra(ICentraService centraService)
        {
            _centraService = centraService;
        }

        [FunctionName(nameof(FillQueueFromCentra))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [Queue("%myqueue-items%"), StorageAccount("AzureWebJobsStorage")] ICollector<string> que,
            ILogger log)
        {
            var products = await _centraService.GetProductSkus();
            var productNumbers = products
                .Where(x => !string.IsNullOrEmpty(x.productNumber))
                .Select(id => id.productNumber.ToString()).ToList();
            foreach (var productNumber in productNumbers)
            {
                que.Add(productNumber);
            }

            return new OkObjectResult("Added to the que.");
        }
    }
}

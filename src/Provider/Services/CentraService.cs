using Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Occtoo.Provider.Centra.Helpers;
using Occtoo.Provider.Centra.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Occtoo.Provider.Centra.Services
{
    public interface ICentraService
    {
        Task<List<T>> GetListOfObjects<T>(string entity, string query, string queryParams = "", int limit = 200);
        Task<List<T>> ExecGraphQLQuery<T>(string entity, string query, string queryParams = "");
        Task<List<ProductGraphQlResponseModel>> GetProductsByProductNumber(string productNumber);
        Task<List<ProductGraphQlResponseModel>> GetProductSkus();
        Task<List<ProductGraphQlResponseModel>> GetProductAsync();
    }
    public class CentraService : ICentraService
    {
        private readonly HttpClient _httpClient;

        public CentraService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {Environment.GetEnvironmentVariable("CentraEnvironmentToken")}");
            _httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("CentraEnvironmentUrl"));
        }

        public async Task<List<ProductGraphQlResponseModel>> GetProductAsync() =>
            await GetListOfObjects<ProductGraphQlResponseModel>("products", "id name productNumber collection { id name uri status } weight { value unit formattedValue } harmonizedCommodityCodeDescription harmonizedCommodityCode countryOfOrigin { name translations { language { name } fields { value } } } folder { name childFolders { name } } displays { markets { name } displayItems { id display { name description } } name id status metaTitle metaKeywords metaDescription description productVariants { productSizes { id SKU description } } } brand { id name } productNumber media { source { url } } variants { prices{ id recommendedRetailPrice{value} price{value formattedValue valueInCents currency{code name}}} internalName supply { supplier { name } } attributes { elements { key description kind ... on AttributeStringElement { value } } description objectType type { name } } name variantNumber translations { language { name countryCode } fields { value } } stock { physicalQuantity availableNowQuantity productSize { id GTIN sizeNumber SKU } warehouse { id name status } } } media { source { url } }", "", 10);

        public async Task<List<ProductGraphQlResponseModel>> GetProductSkus() =>
            await GetListOfObjects<ProductGraphQlResponseModel>("products", "productNumber", "");

        public async Task<List<ProductGraphQlResponseModel>> GetProductsByProductNumber(string productNumber) =>
            await GetListOfObjects<ProductGraphQlResponseModel>("products", "id name productNumber collection { id name uri status } weight { value unit formattedValue } harmonizedCommodityCodeDescription harmonizedCommodityCode countryOfOrigin { name translations { language { name } fields { value } } } folder { name childFolders { name } } displays { markets { name } displayItems { id display { name description } } name id status metaTitle metaKeywords metaDescription description productVariants { productSizes { id SKU description } } } brand { id name } productNumber media { source { url } } variants { prices{ id recommendedRetailPrice{value} price{value formattedValue valueInCents currency{code name}}} internalName supply { supplier { name } } attributes { elements { key description kind ... on AttributeStringElement { value } } description objectType type { name } } name variantNumber translations { language { name countryCode } fields { value } } stock { physicalQuantity availableNowQuantity productSize { id GTIN sizeNumber SKU } warehouse { id name status } } } media { source { url } }", $"where: {{ productNumber: {{ equals: \"{productNumber}\" }} }}", 5);

        public async Task<List<T>> GetListOfObjects<T>(string entity, string query, string queryParams = "", int limit = 200)
        {
            var page = 1;
            var objectList = new List<T>() { };
            List<T> singlePage;
            do
            {
                singlePage = await ExecGraphQLQuery<T>(entity, query, $"limit: {limit}, page: {page} {(queryParams != "" ? "," + queryParams : "")}");
                if (singlePage != null)
                {
                    if (singlePage.Count == 0)
                        break;
                    objectList.AddRange(singlePage);
                    page += 1;
                    Thread.Sleep(100);
                }
            } while (singlePage == null || singlePage.Count == limit);

            return objectList;
        }

        public async Task<List<T>> ExecGraphQLQuery<T>(string entity, string query, string queryParams = "") =>
           await ExecGraphQL<T>($"query {{ {entity} {(queryParams != "" ? $"({queryParams})" : "")} {{ {query}  }}  }}", entity);

        private async Task<List<T>> ExecGraphQL<T>(string queryStr, string arrayName)
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource(new TimeSpan(1, 0, 0));

                var query = new CentraRequest()
                {
                    query = queryStr
                };
                var body = JsonConvert.SerializeObject(query);
                var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
                var response = new CentraQueryResponse();

                var responseMssg = await _httpClient.PostAsync("", content, cancellationTokenSource.Token);
                var jsonStr = await responseMssg.Content.ReadAsStringAsync();
                try
                {
                    response = JsonConvert.DeserializeObject<CentraQueryResponse>(jsonStr);
                }
                catch (JsonReaderException)
                {
                    return null;
                }


                if (response == null || response.errors != null)
                {
                    if (response == null)
                    {
                        throw new Exception($"GraphQL request error on: {arrayName}.");
                    }

                    var errors = JArray.Parse(JsonConvert.SerializeObject(response.errors));
                    var errorMssgs = new List<CentraError>(SerializationHelper.CreateAndInitInstanceList<CentraError>(errors.ToString()));
                    throw new Exception(errorMssgs.Count > 0 ? errorMssgs[0].message : $"GraphQL request error on: {arrayName}.");
                }

                var responseDeserialized = JsonConvert.DeserializeObject<ProductHelperClass<T>>(response.data.ToString());
                return responseDeserialized.products;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}

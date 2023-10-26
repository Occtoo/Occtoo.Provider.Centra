# Centra Provider 
This project gives an example on how to create a provider for Centra to Occtoo onboarding 

**Docs:** https://docs.occtoo.com/

The solution has functions working as shown below:

![Architectual Overview](docs/CentraProvider.png)

FillQueueFromCentra (HttpTrigger) - Called to start moving products from Centra API into the queue

ImportQueuedItemsToOcctoo (QueueTrigger) – Picks up the queue items and create a key value entity (dynamicentity) and sends it to Occtoo onboarding 

## Running the app
### 1. Get the code
Clone the repository and open the solution
### 2. Add a localsettings.json file and fill out the details of your project
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "DataProviderId": "",
    "DataProviderSecret": "",
    "ProductSource": "product",
    "VariantSource": "productVariant",
    "PriceSource": "price",
    "StockSource": "stock",
    "myqueue-items": "myqueue-items",
    "CentraEnvironmentToken": "",
    "CentraEnvironmentUrl": "https://{yourCompany}.centra.com/graphql",
    "AzureFunctionsJobHost__extensions__queues__batchSize": 1
  }
}
```

### 3. Start the application
Hit F5
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace demo.core.Services
{
    public interface IAzureSearchIndexService
    {
        Task Sync(SyncAzureSearchIndexCommand command);
    }

    public class AzureSearchIndexService : IAzureSearchIndexService
    {
         private readonly string _searchServiceName = "codesultingkenshodev";
        private readonly string _adminApiKey = "9B48B3AFD5A1DC83DAA4CE34780302AC";

        public AzureSearchIndexService()
        {
        }

        public async Task Sync(SyncAzureSearchIndexCommand command)
        {
            var serviceClient = new SearchServiceClient(_searchServiceName, new SearchCredentials(_adminApiKey));
            var indexClient = serviceClient.Indexes.GetClient(command.IndexName);

            await SyncAzureSearchIndex(command.IndexName,
                serviceClient,
                indexClient,
                command.IsWholesaleSync,
                command.IndexType,
                command.Models,
                command.Suggesters
            );
        }

        public async Task SyncAzureSearchIndex(string indexName,
            ISearchServiceClient serviceClient,
            ISearchIndexClient indexClient,
            bool isWholesaleSync,
            Type indexDocumentType,
            IEnumerable<dynamic> documents,
            IEnumerable<Suggester> suggesters)
        {
            if (isWholesaleSync)
            {
                //await _log.CaptureAsync($"[{indexName}] Deleting index...\n");
                await DeleteIndexIfExists(indexName, serviceClient);
            }

            //await _log.CaptureAsync($"[{indexName}] Creating or Updating index...\n");
            await CreateOrUpdateAsync(indexName, serviceClient, indexDocumentType, suggesters);

            
            //await _log.CaptureAsync($"[{indexName}] Uploading documents...\n");
            await UploadDocuments(indexName, indexClient, documents);

            //await _log.CaptureAsync($"[{indexName}] Complete. \n");
        }

        #region Sync Index Utility Methods

        private async Task DeleteIndexIfExists(string indexName, ISearchServiceClient serviceClient)
        {
            var existCheck = await serviceClient.Indexes.ExistsWithHttpMessagesAsync(indexName);
            if (existCheck.Body)
            {
                await serviceClient.Indexes.DeleteWithHttpMessagesAsync(indexName);
            }
        }

        // Create an index whose fields correspond to the properties of the Hotel class.
        // The Address property of Hotel will be modeled as a complex field.
        // The properties of the Address class in turn correspond to sub-fields of the Address complex field.
        // The fields of the index are defined by calling the FieldBuilder.BuildForType() method.
        private async Task CreateOrUpdateAsync(string indexName, ISearchServiceClient serviceClient,
            Type indexDocumentType, IEnumerable<Suggester> suggesters)
        {
            var definition = new Index()
            {
                Name = indexName,
                Fields = FieldBuilder.BuildForType(indexDocumentType),
                Suggesters = suggesters?.ToList()
            };

            await serviceClient.Indexes.CreateOrUpdateAsync(definition);
        }

        private async Task UploadDocuments(string indexName, ISearchIndexClient indexClient,
            IEnumerable<dynamic> documents)
        {
            var maxPageCount = 50;
            var pageCount = documents.Count() > maxPageCount ? maxPageCount : documents.Count();
            var pages = documents.Count() / pageCount;
            var currentPage = 1;
            var lastSuccessfulPage = 0;
            IEnumerable<dynamic> documentsInPage;
            //await _log.CaptureAsync($"[{indexName}] Starting to index {pages} pages... ");
            while (currentPage <= pages)
            {
                try
                {
                    documentsInPage = documents.Skip((currentPage * pageCount) - pageCount).Take(pageCount);
                    var indexActions = documentsInPage
                        .Select(IndexAction.MergeOrUpload).ToList();
                    var batch = IndexBatch.New(indexActions);

                    await indexClient.Documents.IndexAsync(batch);
                    lastSuccessfulPage = currentPage;
                    currentPage++;

                    //await _log.CaptureAsync($"[{indexName}] Successfully uploaded {lastSuccessfulPage} of {pages} pages... ");
                }
                catch (Exception ex)
                {
                    //await _log.CaptureAsync(ex);

                    if (ex is IndexBatchException e)
                    {
                        // When a service is under load, indexing might fail for some documents in the batch. 
                        // Depending on your application, you can compensate by delaying and retrying. 
                        // For this simple demo, we just log the failed document keys and continue.
//                        await _log.CaptureAsync(
//                            string.Format($"[{indexName}] Failed to index some of the documents: {0}",
//                                String.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key))));
                    }
                }
            }
        }

        #endregion
    }
}
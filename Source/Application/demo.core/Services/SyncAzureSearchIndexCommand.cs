using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Search.Models;

namespace demo.core.Services
{
    public class SyncAzureSearchIndexCommand
    {

        private SyncAzureSearchIndexCommand(string indexName, Type indexType, IEnumerable<dynamic> models, IEnumerable<Suggester> suggesters,
            bool isWholesaleSync)
        {
            IndexName = indexName;
            IndexType = indexType;
            Models = models;
            Suggesters = suggesters;
            IsWholesaleSync = isWholesaleSync;
        }

        public string IndexName { get; }
        public Type IndexType { get; }
        public IEnumerable<dynamic> Models { get; }
        public bool IsWholesaleSync { get; }
        public IEnumerable<Suggester> Suggesters { get;  }

        public static SyncAzureSearchIndexCommand For<T>(string indexName, IEnumerable<T> models, IEnumerable<Suggester> suggesters, bool isWholesaleSync)
        {
            return new SyncAzureSearchIndexCommand(indexName, typeof(T), models as IEnumerable<dynamic>, suggesters, isWholesaleSync);
        }
    }
    
    public class SyncAzureSearchIndexExecutor 
    {
        private readonly IAzureSearchIndexService _azureSearchIndex;

        public SyncAzureSearchIndexExecutor(IAzureSearchIndexService azureSearchIndex)
        {
            _azureSearchIndex = azureSearchIndex;
        }


        public async Task Execute(SyncAzureSearchIndexCommand command)
        {
            await _azureSearchIndex.Sync(command);
        }
    }
}
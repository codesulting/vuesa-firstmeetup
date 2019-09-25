using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using demo.core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Spatial;
using Newtonsoft.Json;

namespace demo.functions
{
    public class AzureIndexSyncFunctions : InstanceFunctionBase
    {
        public AzureIndexSyncFunctions()
            : base()
        {
        }

//        [FunctionName(nameof(AzureIndexSyncFunctions) + nameof(TimerFunction))]
//        public async Task TimerFunction([TimerTrigger("0 */5 * * * *")] TimerInfo timerInfo, ILogger log,
//            ExecutionContext context)
//        {
//            await SyncFunction(log);
//        }

        [FunctionName(nameof(AzureIndexSyncFunctions) + nameof(PostFunction))]
        public async Task<IActionResult> PostFunction(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route =
                nameof(AzureIndexSyncFunctions) + "/" + nameof(PostFunction))]
            HttpRequest req, ILogger log, ExecutionContext context)
        {
            return await SyncFunction(log);
        }

        private async Task<IActionResult> SyncFunction(ILogger log)
        {
            var commandSender = new SyncAzureSearchIndexExecutor(new AzureSearchIndexService());
            List<StoreItem> models = new List<StoreItem>()
            {
                new StoreItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Halo Lens Headset",
                    You_Can_Buy_This_Online = true,
                    Image = "some_image",
                    Latitude = 31.9375,
                    Longitude = -102.3940,
                    Manufacturer = "Microsoft"
                },
                new StoreItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Samsung phone",
                    You_Can_Buy_This_Online = true,
                    Image = "some_image",
                    Latitude = 31.9375,
                    Longitude = -102.3940,
                    Manufacturer = "Samsung"
                },
                new StoreItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "4k UHD Monitor",
                    You_Can_Buy_This_Online = false,
                    Image = "some_image",
                    Latitude = 31.9375,
                    Longitude = -102.3940,
                    Manufacturer = "Samsung"
                },
                new StoreItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "iPhone with 3 cameras",
                    You_Can_Buy_This_Online = false,
                    Image = "some_image",
                    Latitude = 31.9375,
                    Longitude = -102.3940,
                    Manufacturer = "Apple"
                },
                new StoreItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Windows Phone - not really",
                    You_Can_Buy_This_Online = true,
                    Image = "some_image",
                    Latitude = 31.9375,
                    Longitude = -102.3940,
                    Manufacturer = "Microsoft"
                }
            };
            IEnumerable<Suggester> suggesters = new List<Suggester>()
            {
                new Suggester()
                {
                    Name = "sg",
                    SourceFields = new[] {"Name", "Manufacturer"}
                }
            };
            await commandSender.Execute(SyncAzureSearchIndexCommand.For("demo-store-index", models, suggesters, true));

            return null;
        }
    }
    //https://codesultingkenshodev.search.windows.net/indexes/demo-store-index/docs/suggest?api-version=2019-05-06&search=micro&$top=3&suggesterName=sg&searchFields=ZIPBR&$select=ZIPBR&fuzzy=true&highlightPreTag=<b>&highlightPostTag=</b>
    //https://codesultingkenshodev.search.windows.net/indexes/demo-store-index/docs/autocomplete?api-version=2019-05-06&search=uhd&suggesterName=sg&searchFields=Name&highlightPreTag=<b>&highlightPostTag=</b>&AutocompleteMode=TwoTerms
    public class StoreItem
    {
        [System.ComponentModel.DataAnnotations.Key, IsFacetable, IsFilterable]
        [JsonProperty(PropertyName = "PublicID")]
        public string Id { get; set; }

        [IsRetrievable(false), IsFilterable, JsonProperty(PropertyName = "Enabled")]
        public bool You_Can_Buy_This_Online { get; set; }

        [IsRetrievable(true), IsFilterable, IsSortable, IsSearchable]
        public string Name { get; set; }

        [IsRetrievable(true), IsFilterable, IsSortable]
        public string Image { get; set; }

        [JsonIgnore] public double? Latitude { get; set; }
        [JsonIgnore] public double? Longitude { get; set; }

        [IsRetrievable(true), IsFilterable, IsSortable]
        public GeographyPoint Location
        {
            get
            {
                return Latitude.HasValue && Longitude.HasValue
                    ? GeographyPoint.Create(Latitude.Value, Longitude.Value)
                    : null;
            }
            set
            {
                Latitude = value?.Latitude;
                Longitude = value?.Longitude;
            }
        }

        [IsRetrievable(true), IsFilterable, IsFacetable, IsSearchable]
        public string Manufacturer { get; set; }
    }
}
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace nTipBot.Models.OpenApiNkn
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class NetworkStatusModel
    {
        [JsonProperty("totalNodes")]
        public int TotalNodes;

        [JsonProperty("totalCountries")]
        public int TotalCountries;

        [JsonProperty("totalProviders")]
        public int TotalProviders;

        [JsonProperty("updatedTime")]
        public string UpdatedTime;
    }


}

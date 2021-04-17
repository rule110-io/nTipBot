using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace nTipBot.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class City
    {
        [JsonProperty("Country")]
        public string Country { get; set; }

        [JsonProperty("CountryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("City")]
        public string CityName { get; set; }

        [JsonProperty("IPAddress")]
        public string IPAddress { get; set; }

        [JsonProperty("Lat")]
        public double Lat { get; set; }

        [JsonProperty("Lon")]
        public double Lon { get; set; }

        [JsonProperty("Count")]
        public int Count { get; set; }
    }

    public class Summary
    {
        [JsonProperty("Country")]
        public string Country { get; set; }

        [JsonProperty("Count")]
        public int Count { get; set; }

        [JsonProperty("Lat")]
        public double Lat { get; set; }

        [JsonProperty("Lon")]
        public double Lon { get; set; }

        [JsonProperty("Cities")]
        public List<City> Cities { get; set; }
    }

    public class Payload
    {
        [JsonProperty("summary")]
        public List<Summary> Summary { get; set; }

        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }
    }

    public class NKNGeoSummary
    {
        [JsonProperty("Payload")]
        public Payload Payload { get; set; }

        [JsonProperty("Timestamp")]
        public string Timestamp { get; set; }

        [JsonProperty("ApiVersion")]
        public string ApiVersion { get; set; }
    }


}

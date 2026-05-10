using System.Collections.Generic;
using Newtonsoft.Json;

namespace ArendaDesktop.Models
{
    public class PropertyList
    {
        [JsonProperty("items")]
        public List<Property> Items { get; set; } = new List<Property>();

        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }
    }
}

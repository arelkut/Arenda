using System.Collections.Generic;
using Newtonsoft.Json;

namespace ArendaDesktop.Models
{
    public class PropertyMedia
    {
        [JsonProperty("media_id")]
        public int MediaId { get; set; }

        [JsonProperty("media_type")]
        public string MediaType { get; set; }

        [JsonProperty("file_path")]
        public string FilePath { get; set; }

        [JsonProperty("is_main")]
        public bool IsMain { get; set; }
    }

    public class Property
    {
        [JsonProperty("property_id")]
        public int PropertyId { get; set; }

        [JsonProperty("landlord_id")]
        public int LandlordId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("district")]
        public string District { get; set; }

        [JsonProperty("property_type")]
        public string PropertyType { get; set; }

        [JsonProperty("area")]
        public double Area { get; set; }

        [JsonProperty("rooms")]
        public int? Rooms { get; set; }

        [JsonProperty("floor")]
        public int? Floor { get; set; }

        [JsonProperty("total_floors")]
        public int? TotalFloors { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("is_active")]
        public bool IsActive { get; set; }

        [JsonProperty("created_date")]
        public string CreatedDate { get; set; }

        [JsonProperty("status_id")]
        public int StatusId { get; set; }

        [JsonProperty("media")]
        public List<PropertyMedia> Media { get; set; } = new List<PropertyMedia>();

        public string StatusText
        {
            get
            {
                switch (StatusId)
                {
                    case 1: return "На модерации";
                    case 2: return "Активно";
                    case 3: return "Отклонено";
                    case 4: return "Архив";
                    default: return "Неизвестно";
                }
            }
        }

        public string PriceFormatted => $"{Price:N0} ₽/мес";

        public string AreaFormatted => $"{Area:N1} м²";

        public string FloorInfo =>
            Floor.HasValue && TotalFloors.HasValue
                ? $"{Floor}/{TotalFloors} этаж"
                : Floor.HasValue
                    ? $"{Floor} этаж"
                    : "";

        public string RoomsText =>
            Rooms.HasValue ? $"{Rooms} комн." : "";

        public string MainPhotoUrl
        {
            get
            {
                if (Media == null || Media.Count == 0) return null;
                foreach (var m in Media)
                {
                    if (m.IsMain) return m.FilePath;
                }
                return Media[0].FilePath;
            }
        }
    }

    public class PropertyCreate
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("district")]
        public string District { get; set; }

        [JsonProperty("property_type")]
        public string PropertyType { get; set; }

        [JsonProperty("area")]
        public double Area { get; set; }

        [JsonProperty("rooms")]
        public int? Rooms { get; set; }

        [JsonProperty("floor")]
        public int? Floor { get; set; }

        [JsonProperty("total_floors")]
        public int? TotalFloors { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }
    }
}

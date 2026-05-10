using System.Collections.Generic;
using Newtonsoft.Json;

namespace ArendaDesktop.Models
{
    public class UserInfo
    {
        [JsonProperty("user_id")]
        public int UserId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("registration_date")]
        public string RegistrationDate { get; set; }

        [JsonProperty("is_active")]
        public bool IsActive { get; set; }

        [JsonProperty("roles")]
        public List<string> Roles { get; set; } = new List<string>();

        public string RolesText
        {
            get
            {
                var parts = new List<string>();
                foreach (var role in Roles)
                {
                    switch (role)
                    {
                        case "admin": parts.Add("Админ"); break;
                        case "landlord": parts.Add("Арендодатель"); break;
                        case "tenant": parts.Add("Арендатор"); break;
                        default: parts.Add(role); break;
                    }
                }
                return string.Join(", ", parts);
            }
        }

        public string StatusText => IsActive ? "Активен" : "Заблокирован";
    }
}

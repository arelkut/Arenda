using System.Collections.Generic;
using Newtonsoft.Json;

namespace ArendaDesktop.Models
{
    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("user_id")]
        public int UserId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("roles")]
        public List<string> Roles { get; set; } = new List<string>();
    }
}

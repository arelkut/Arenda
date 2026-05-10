using System.Collections.Generic;

namespace ArendaDesktop.Helpers
{
    public static class AuthHelper
    {
        public static string Token { get; set; }
        public static int UserId { get; set; }
        public static string Email { get; set; }
        public static List<string> Roles { get; set; } = new List<string>();

        public static bool IsAuthenticated => !string.IsNullOrEmpty(Token);

        public static bool IsAdmin => Roles.Contains("admin");

        public static bool IsLandlord => Roles.Contains("landlord");

        public static bool IsTenant => Roles.Contains("tenant");

        public static void SetAuth(Models.TokenResponse response)
        {
            Token = response.AccessToken;
            UserId = response.UserId;
            Email = response.Email;
            Roles = response.Roles ?? new List<string>();
        }

        public static void Clear()
        {
            Token = null;
            UserId = 0;
            Email = null;
            Roles = new List<string>();
        }
    }
}

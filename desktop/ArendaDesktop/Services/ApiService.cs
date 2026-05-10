using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using ArendaDesktop.Helpers;
using ArendaDesktop.Models;
using Newtonsoft.Json;

namespace ArendaDesktop.Services
{
    public static class ApiService
    {
        private static readonly HttpClient Client = new HttpClient();
        private static string _baseUrl = "http://localhost:8000/api";

        public static string BaseUrl
        {
            get => _baseUrl;
            set => _baseUrl = value.TrimEnd('/');
        }

        private static string Get(string path)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _baseUrl + path);
            if (AuthHelper.IsAuthenticated)
            {
                request.Headers.Add("Authorization", "Bearer " + AuthHelper.Token);
            }
            var response = Client.SendAsync(request).GetAwaiter().GetResult();
            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                var error = TryGetErrorDetail(content);
                throw new Exception(error ?? $"Ошибка {(int)response.StatusCode}: {response.ReasonPhrase}");
            }
            return content;
        }

        private static string Post(string path, object body)
        {
            var json = JsonConvert.SerializeObject(body);
            var request = new HttpRequestMessage(HttpMethod.Post, _baseUrl + path)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            if (AuthHelper.IsAuthenticated)
            {
                request.Headers.Add("Authorization", "Bearer " + AuthHelper.Token);
            }
            var response = Client.SendAsync(request).GetAwaiter().GetResult();
            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                var error = TryGetErrorDetail(content);
                throw new Exception(error ?? $"Ошибка {(int)response.StatusCode}: {response.ReasonPhrase}");
            }
            return content;
        }

        private static string Put(string path)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, _baseUrl + path);
            if (AuthHelper.IsAuthenticated)
            {
                request.Headers.Add("Authorization", "Bearer " + AuthHelper.Token);
            }
            var response = Client.SendAsync(request).GetAwaiter().GetResult();
            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                var error = TryGetErrorDetail(content);
                throw new Exception(error ?? $"Ошибка {(int)response.StatusCode}: {response.ReasonPhrase}");
            }
            return content;
        }

        private static void Delete(string path)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, _baseUrl + path);
            if (AuthHelper.IsAuthenticated)
            {
                request.Headers.Add("Authorization", "Bearer " + AuthHelper.Token);
            }
            var response = Client.SendAsync(request).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var error = TryGetErrorDetail(content);
                throw new Exception(error ?? $"Ошибка {(int)response.StatusCode}: {response.ReasonPhrase}");
            }
        }

        private static string TryGetErrorDetail(string json)
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                if (obj != null && obj.ContainsKey("detail"))
                    return obj["detail"];
            }
            catch { }
            return null;
        }

        // ===== Auth =====

        public static TokenResponse Login(string email, string password)
        {
            var body = new { email, password };
            var json = Post("/auth/login", body);
            return JsonConvert.DeserializeObject<TokenResponse>(json);
        }

        public static TokenResponse Register(string email, string password, string firstName = null,
            string lastName = null, string phone = null)
        {
            var body = new { email, password, first_name = firstName, last_name = lastName, phone };
            var json = Post("/auth/register", body);
            return JsonConvert.DeserializeObject<TokenResponse>(json);
        }

        public static UserInfo GetMe()
        {
            var json = Get("/auth/me?token=" + AuthHelper.Token);
            return JsonConvert.DeserializeObject<UserInfo>(json);
        }

        // ===== Properties =====

        public static PropertyList GetProperties(int page = 1, int size = 12, string search = null,
            string city = null, string propertyType = null, int? rooms = null,
            double? minPrice = null, double? maxPrice = null)
        {
            var parts = new List<string>
            {
                "page=" + page,
                "size=" + size
            };
            if (!string.IsNullOrEmpty(search)) parts.Add("search=" + Uri.EscapeDataString(search));
            if (!string.IsNullOrEmpty(city)) parts.Add("city=" + Uri.EscapeDataString(city));
            if (!string.IsNullOrEmpty(propertyType)) parts.Add("property_type=" + Uri.EscapeDataString(propertyType));
            if (rooms.HasValue) parts.Add("rooms=" + rooms.Value);
            if (minPrice.HasValue) parts.Add("min_price=" + minPrice.Value);
            if (maxPrice.HasValue) parts.Add("max_price=" + maxPrice.Value);

            var query = string.Join("&", parts);
            var json = Get("/properties?" + query);
            return JsonConvert.DeserializeObject<PropertyList>(json);
        }

        public static PropertyList GetAllPropertiesForAdmin(int page = 1, int size = 50)
        {
            var json = Get($"/properties/admin/all?page={page}&size={size}");
            return JsonConvert.DeserializeObject<PropertyList>(json);
        }

        public static Property GetProperty(int id)
        {
            var json = Get("/properties/" + id);
            return JsonConvert.DeserializeObject<Property>(json);
        }

        public static Property CreateProperty(PropertyCreate data)
        {
            var json = Post("/properties", data);
            return JsonConvert.DeserializeObject<Property>(json);
        }

        public static void UpdatePropertyStatus(int propertyId, int statusId)
        {
            Put($"/properties/{propertyId}?status_id={statusId}");
        }

        public static void DeleteProperty(int propertyId)
        {
            Delete("/properties/" + propertyId);
        }
    }
}

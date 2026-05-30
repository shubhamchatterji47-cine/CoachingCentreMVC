using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace CoachingMVC.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _ctx;

        public ApiService(HttpClient http, IHttpContextAccessor ctx, IConfiguration config)
        {
            _http = http;
            _ctx = ctx;
            var baseUrl = config["ApiBaseUrl"] ?? "http://localhost:5107/api/";
            _http.BaseAddress = new Uri(baseUrl);
        }

        private string? GetToken() =>
            _ctx.HttpContext?.Session.GetString("Token");

        private HttpRequestMessage CreateRequest(
            HttpMethod method, string endpoint, HttpContent? content = null)
        {
            var request = new HttpRequestMessage(method, endpoint);
            var token = GetToken();
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            if (content != null)
                request.Content = content;
            return request;
        }

        private string ParseError(string raw, System.Net.HttpStatusCode statusCode)
        {
            if (!string.IsNullOrWhiteSpace(raw))
            {
                try
                {
                    dynamic? err = JsonConvert.DeserializeObject(raw);
                    string? msg = err?.message?.ToString() ?? err?.title?.ToString();
                    if (!string.IsNullOrEmpty(msg)) return msg;
                }
                catch { }
                return raw.Length > 150 ? raw[..150] : raw;
            }
            return statusCode switch
            {
                System.Net.HttpStatusCode.Unauthorized => "Unauthorized — please log out and log in again.",
                System.Net.HttpStatusCode.Forbidden => "You do not have permission to perform this action.",
                System.Net.HttpStatusCode.NotFound => "The requested resource was not found.",
                System.Net.HttpStatusCode.MethodNotAllowed => "Method not allowed.",
                System.Net.HttpStatusCode.BadRequest => "Bad request — check your input.",
                _ => $"Error {(int)statusCode}"
            };
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            try
            {
                var request = CreateRequest(HttpMethod.Get, endpoint);
                var resp = await _http.SendAsync(request);
                if (!resp.IsSuccessStatusCode) return default;
                var json = await resp.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(json)) return default;
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch { return default; }
        }

        public async Task<(bool Success, string? Message, T? Data)> PostAsync<T>(
            string endpoint, object body)
        {
            try
            {
                var json = JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var request = CreateRequest(HttpMethod.Post, endpoint, content);
                var resp = await _http.SendAsync(request);
                var raw = await resp.Content.ReadAsStringAsync();

                if (resp.IsSuccessStatusCode)
                {
                    T? data = default;
                    if (!string.IsNullOrWhiteSpace(raw))
                        try { data = JsonConvert.DeserializeObject<T>(raw); } catch { }
                    return (true, null, data);
                }
                return (false, ParseError(raw, resp.StatusCode), default);
            }
            catch (Exception ex)
            {
                return (false, $"Connection error: {ex.Message}", default);
            }
        }

        public async Task<(bool Success, string? Message)> PostAsync(
            string endpoint, object body)
        {
            var (success, msg, _) = await PostAsync<object>(endpoint, body);
            return (success, msg);
        }

        public async Task<(bool Success, string? Message)> PutAsync(
            string endpoint, object body)
        {
            try
            {
                var json = JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var request = CreateRequest(HttpMethod.Put, endpoint, content);
                var resp = await _http.SendAsync(request);
                var raw = await resp.Content.ReadAsStringAsync();
                if (resp.IsSuccessStatusCode) return (true, null);
                return (false, ParseError(raw, resp.StatusCode));
            }
            catch (Exception ex)
            {
                return (false, $"Connection error: {ex.Message}");
            }
        }

        public async Task<(bool Success, string? Message)> DeleteAsync(string endpoint)
        {
            try
            {
                var request = CreateRequest(HttpMethod.Delete, endpoint);
                var resp = await _http.SendAsync(request);
                var raw = await resp.Content.ReadAsStringAsync();
                if (resp.IsSuccessStatusCode) return (true, null);
                return (false, ParseError(raw, resp.StatusCode));
            }
            catch (Exception ex)
            {
                return (false, $"Connection error: {ex.Message}");
            }
        }
    }
}
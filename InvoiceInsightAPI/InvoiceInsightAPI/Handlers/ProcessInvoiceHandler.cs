using InvoiceInsightAPI.Handlers.Interfaces;
using InvoiceInsightAPI.Models;
using InvoiceInsightAPI.Models.Request;
using InvoiceInsightAPI.Models.Response;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace InvoiceInsightAPI.Handlers
{
    public class ProcessInvoiceHandler(IConfiguration configuration) : IProcessInvoiceHandler
    {
        public async Task<ApiResponse<ProcessInvoiceResponse>> ProcessInvoiceAsync(ProcessInvoiceRequest req)
        {
            var prompt = configuration["Gemini:PromptProcessInvoice"];
            var apiKey = configuration["Gemini:ApiKey"];
            var url = configuration["Gemini:Url"];

            if (string.IsNullOrEmpty(prompt) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("Prompt or api key or url are not configured!");
            }
            
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new { text = prompt }, 
                            new
                            {
                                inlineData = new
                                {
                                    mimeType = req.ContentType,
                                    data = req.Base64Content
                                }
                            }
                        }
                    }
                }
            };
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(requestBody, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                Encoding.UTF8,
                "application/json"
            );
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("x-goog-api-key", apiKey);

            try
            {
                var geminiResponse = await httpClient.PostAsync(url, jsonContent);
                geminiResponse.EnsureSuccessStatusCode();
                var responseBody = await geminiResponse.Content.ReadAsStringAsync();
                var geminiResponseObj = JsonSerializer.Deserialize<GeminiResponse>(responseBody);
                var text = geminiResponseObj?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;
                
                if (string.IsNullOrEmpty(text)) throw new JsonException("Text is empty!");
                
                var cleanedJson = Regex.Replace(text, @"^```json|```$", "", RegexOptions.Multiline).Trim();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var invoiceData = JsonSerializer.Deserialize<InvoiceData>(cleanedJson, options);

                return new ApiResponse<ProcessInvoiceResponse>()
                {
                    HttpStatusCode = HttpStatusCode.OK,
                    Result = new ProcessInvoiceResponse()
                    {
                        InvoiceData = invoiceData
                    },
                    IsSuccess = true
                };
            }
            catch (HttpRequestException)
            {
                return new ApiResponse<ProcessInvoiceResponse>()
                {
                    HttpStatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = ["Http request error"]
                };
            }            
            catch (JsonException)
            {
                return new ApiResponse<ProcessInvoiceResponse>()
                {
                    HttpStatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = ["Error parsing json"]
                };
            }            
            catch (Exception)
            {
                return new ApiResponse<ProcessInvoiceResponse>()
                {
                    HttpStatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = ["Unexpected error"]
                };
            }
        }
    }
}

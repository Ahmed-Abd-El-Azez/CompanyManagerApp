using CompanyMaster;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CompanyWebApplicationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiChatController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AiChatController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequestDto request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("N8nClient");
                var aiEndpointUrl = _configuration["N8nSettings:WebhookUrl"];

                if (string.IsNullOrEmpty(aiEndpointUrl))
                {
                    return StatusCode(500, new ChatResponseDto { Success = false, ErrorMessage = "Webhook URL is missing in appsettings.json." });
                }
                

                var response = await client.PostAsJsonAsync(aiEndpointUrl, request);

                if (!response.IsSuccessStatusCode)
                {
                    var rawN8nError = await response.Content.ReadAsStringAsync();

                    return StatusCode((int)response.StatusCode, new ChatResponseDto
                    {
                        Success = false,
                        ErrorMessage = $"n8n HTTP {(int)response.StatusCode} Error: {rawN8nError}"
                    });
                }

                // Read raw string first to avoid JSON deserialization crashes
                var rawJson = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<ChatResponseDto>(rawJson, options);

                return Ok(result ?? new ChatResponseDto { Output = rawJson });
            }
            catch (Exception ex)
            {
                // Catch internal API failure and return details instead of throwing a generic 500
                return StatusCode(500, new ChatResponseDto
                {
                    Success = false,
                    ErrorMessage = $"Internal Web API Exception: {ex.Message} | StackTrace: {ex.StackTrace}"
                });
            }
        }
    }
}
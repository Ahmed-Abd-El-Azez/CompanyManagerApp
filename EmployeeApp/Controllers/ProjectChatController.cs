using CompanyMaster;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApp.Controllers
{
    public class ProjectChatController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProjectChatController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequestDto request)
        {
            var client = _httpClientFactory.CreateClient("EmployeeAPI"); // Uses base URL http://companymasterapi.runasp.net/
            var response = await client.PostAsJsonAsync("AiChat/send", request);

            if (!response.IsSuccessStatusCode) {
                return StatusCode((int)response.StatusCode, new
                {
                    success = false,
                    errorMessage = $"Web API returned Status {(int)response.StatusCode} ({response.ReasonPhrase})"
                });
            }

            var data = await response.Content.ReadFromJsonAsync<ChatResponseDto>();
            return Ok(data);
        }
    }
}

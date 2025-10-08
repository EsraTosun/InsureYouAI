using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InsureYouAI.Controllers
{
    public class ImageAIController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ImageAIController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult CreateImageWithOpenAI()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateImageWithOpenAI(string prompt)
        {
            var apiKey = "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";
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var requestData = new
            {
                prompt = prompt,
                n = 1,
                size = "512x512"
            };

            var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8,
            "application/json");

            var response = await client.PostAsync("https://api.openai.com/v1/images/generations", content);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.error = "OpenAI Hatası: " + await response.Content.ReadAsStringAsync();
                return View();
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonDocument.Parse(json);
            var imageUrl = result.RootElement.GetProperty("data")[0].GetProperty("url").GetString();

            return View(model: imageUrl);
        }
    }
}

//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
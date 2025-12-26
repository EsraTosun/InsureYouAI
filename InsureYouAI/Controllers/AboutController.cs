using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace InsureYouAI.Controllers
{
    public class AboutController : Controller
    {
        private readonly InsureContext _context;
        public AboutController(InsureContext context)
        {
            _context = context;
        }
        public IActionResult AboutList()
        {
            ViewBag.ControllerName = "Hakkımızda";
            ViewBag.PageName = "Mevcut Hakkımızda Yazısı";
            var values = _context.Abouts.ToList();
            return View(values);
        }

        [HttpGet]
        public IActionResult CreateAbout()
        {
            ViewBag.ControllerName = "Hakkımızda";
            ViewBag.PageName = "Yeni Hakkımızda Yaazı Girişi (Tema Bütünlüğünü Korumak İçin 1 Adet Hakkımızda Yazısı Giriniz)";
            return View();
        }

        [HttpPost]
        public IActionResult CreateAbout(About about)
        {
            _context.Abouts.Add(about);
            _context.SaveChanges();
            return RedirectToAction("AboutList");
        }

        public IActionResult DeleteAbout(int id)
        {
            var value = _context.Abouts.Find(id);
            _context.Abouts.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("AboutList");
        }

        [HttpGet]
        public IActionResult UpdateAbout(int id)
        {
            ViewBag.ControllerName = "Hakkımızda";
            ViewBag.PageName = "Hakkımızda Yazı Güncelleme Sayfası";
            var value = _context.Abouts.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateAbout(About about)
        {
            _context.Abouts.Update(about);
            _context.SaveChanges();
            return RedirectToAction("AboutList");
        }

        [HttpGet]
        public async Task<IActionResult> CreateAboutWithGoogleGemini()
        {
            var apiKey = "AIzaSyAfU0XAcaIxgkRXe9sT5BwqbaN0LJps16s";
            var model = "gemini-2.5-flash";
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts=new[]
                        {
                            new
                            {
                                text="Kurumsal bir sigorta firması için etkileyici, güven verici ve profesyonel bir 'Hakkımızda' yazısı oluştur."
                            }
                        }
                    }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(url, content);
            var responseJson = await response.Content.ReadAsStringAsync();

            using var jsonDoc = JsonDocument.Parse(responseJson);
            var aboutText = jsonDoc.RootElement
                                 .GetProperty("candidates")[0]
                                 .GetProperty("content")
                                 .GetProperty("parts")[0]
                                 .GetProperty("text")
                                 .GetString();

            ViewBag.value = aboutText;

            return View();
        }
    }
}

using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace InsureYouAI.Controllers
{
    public class AboutItemController : Controller
    {
        private readonly InsureContext _context;
        public AboutItemController(InsureContext context)
        {
            _context = context;
        }
        public IActionResult AboutItemList()
        {
            ViewBag.ControllerName = "Hakkımızda Ögeleri";
            ViewBag.PageName = "Mevcut Hakkımızda Ögeleri";
            var values = _context.AboutItems.ToList();
            return View(values);
        }

        [HttpGet]
        public IActionResult CreateAboutItem()
        {
            ViewBag.ControllerName = "Hakkımızda Ögeleri";
            ViewBag.PageName = "Yeni Hakkımızda Öge Girişi";
            return View();
        }

        [HttpPost]
        public IActionResult CreateAboutItem(AboutItem aboutItem)
        {
            _context.AboutItems.Add(aboutItem);
            _context.SaveChanges();
            return RedirectToAction("AboutItemList");
        }

        [HttpGet]
        public IActionResult UpdateAboutItem(int id)
        {
            ViewBag.ControllerName = "Hakkımızda";
            ViewBag.PageName = "Hakkımızda Ögeleri Güncelleme Sayfası";
            var value = _context.AboutItems.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateAboutItem(AboutItem aboutItem)
        {
            _context.AboutItems.Update(aboutItem);
            _context.SaveChanges();
            return RedirectToAction("AboutItemList");
        }

        public IActionResult DeleteAboutItem(int id)
        {
            var value = _context.AboutItems.Find(id);
            _context.AboutItems.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("AboutItemList");
        }

        [HttpGet]
        public async Task<IActionResult> CreateAboutItemWithGoogleGemini()
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
                                text="Kurumsal bir sigorta firması için etkileyici, güven verici ve profesyonel bir 'Hakkımızda alanları (about item)' yazısı oluştur. Örneğin: 'Geleceğinizi güvence altına alan kapsamlı sigorta çözümleri sunuyoruz.' şeklinde veya bunun gibi ve buna benzer daha zengin içerikler gelsin. En az 10 tane item istiyorum."
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

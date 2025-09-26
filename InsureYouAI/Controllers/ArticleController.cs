using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace InsureYouAI.Controllers
{
    public class ArticleController : Controller
    {
        private readonly InsureContext _context;
        public ArticleController(InsureContext context)
        {
            _context = context;
        }
        public IActionResult ArticleList()
        {
            var values = _context.Articles.ToList();
            return View(values);
        }

        [HttpGet]
        public IActionResult CreateArticle()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateArticle(Article article)
        {
            article.CreatedDate = DateTime.Now;
            _context.Articles.Add(article);
            _context.SaveChanges();
            return RedirectToAction("ArticleList");
        }

        [HttpGet]
        public IActionResult UpdateArticle(int id)
        {
            var value = _context.Articles.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateArticle(Article article)
        {
            _context.Articles.Update(article);
            _context.SaveChanges();
            return RedirectToAction("ArticleList");
        }

        public IActionResult DeleteArticle(int id)
        {
            var value = _context.Articles.Find(id);
            _context.Articles.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("ArticleList");
        }

        [HttpGet]
        public IActionResult CreateArticleWithOpenAI()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateArticleWithOpenAI(string prompt)
        {
            var apiKey = "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";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var requestData = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new {role="system",content="Sen bir sigorta şirketi için çalışan, içerik yazarlığı yapan bir yapay zekasın. Kullanıcının verdiği özet ve anahtar kelimelere göre, sigortacılık sektörüyle ilgili makale üret. En az 3000 karakter olsun."},
                    new{role="user",content=prompt }
                },
                temperature = 0.7
            };

            var response = await client.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestData);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<OpenAIResponse>();
                var content = result.choices[0].message.content;
                ViewBag.article = content;
            }
            else
            {
                ViewBag.article = "Bir hata oluştu: " + response.StatusCode;
            }
            return View();
        }

        public class OpenAIResponse
        {
            public List<Choice> choices { get; set; }
        }
        public class Choice
        {
            public Message message { get; set; }
        }
        public class Message
        {
            public string role { get; set; }
            public string content { get; set; }
        }
    }
}

//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
using InsureYouAI.Context;
using InsureYouAI.Entities;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace InsureYouAI.Controllers
{
    public class DefaultController : Controller
    {
        private readonly InsureContext _context;
        public DefaultController(InsureContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public PartialViewResult SendMessage()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(Message message)
        {
            message.SendDate = DateTime.Now;
            message.IsRead = false;
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            string textContent = null;

            #region OpenAI_Analiz
            try
            {
                string apiKey = "api-key";
                string prompt = $"Sen bir sigorta firmasının müşteri iletişim asistanısın.\r\n\r\n" +
                                $"Kurumsal ama samimi, net ve anlaşılır bir dille yaz.\r\n" +
                                $"Yanıtlarını 2–3 paragrafla sınırla.\r\n" +
                                $"Eksik bilgi varsa kibarca talep et.\r\n" +
                                $"Fiyat, ödeme, teminat gibi kritik konularda kesin rakam verme, müşteri temsilcisine yönlendir.\r\n" +
                                $"Hasar ve sağlık gibi hassas durumlarda empati kur.\r\n" +
                                $"Cevaplarını teşekkür ve iyi dilekle bitir.\r\n" +
                                $"Kullanıcının mesajı: '{message.MessagetDetail}'";

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                new { role = "user", content = prompt }
            },
                    max_tokens = 1000,
                    temperature = 0.5
                };

                var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", jsonContent);
                var responseString = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine("OpenAI API response:");
                System.Diagnostics.Debug.WriteLine(responseString);

                if (!response.IsSuccessStatusCode)
                {
                    textContent = $"OpenAI servisi şu anda yanıt vermiyor. Hata kodu: {response.StatusCode}";
                }
                else
                {
                    var json = JsonNode.Parse(responseString);
                    textContent = json?["choices"]?[0]?["message"]?["content"]?.ToString()
                                  ?? "OpenAI yanıtı alınamadı.";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("OpenAI exception: " + ex);
                textContent = "AI servisine ulaşılırken hata oluştu.";
            }
            #endregion

            // Email gönderme ve DB kayıt kısmı aynen kalabilir
            #region Email_Gönderme
            try
            {
                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress("InsureYouAI Admin", "esratosun171@gmail.com"));
                mimeMessage.To.Add(new MailboxAddress("User", message.Email));
                mimeMessage.Subject = "InsureYouAI Email Yanıtı";

                var bodyBuilder = new BodyBuilder { TextBody = textContent ?? "Yanıt alınamadı." };
                mimeMessage.Body = bodyBuilder.ToMessageBody();

                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                smtp.Connect("smtp.gmail.com", 587, false);
                smtp.Authenticate("esratosun171@gmail.com", "nywj bbyu hpuu zykk");
                smtp.Send(mimeMessage);
                smtp.Disconnect(true);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("SMTP Hatası: " + e);
            }
            #endregion

            #region AIMessage_DbKayıt
            try
            {
                var aiMessage = new ClaudeAIMessage
                {
                    MessageDetail = textContent,
                    ReceiveEmail = message.Email,
                    ReceiveNameSurname = message.NameSurname,
                    SendDate = DateTime.Now
                };
                _context.ClaudeAIMessages.Add(aiMessage);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("DB kaydetme hatası: " + ex);
            }
            #endregion

            return RedirectToAction("Index");
        }


        public PartialViewResult SubscribeEmail()
        {
            return PartialView();
        }

        [HttpPost]
        public IActionResult SubscribeEmail(string email)
        {
            return View();
        }
    }
}
//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

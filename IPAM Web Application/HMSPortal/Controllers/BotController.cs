// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.22.0

using EchoBot.Model;
using HMSPortal.Application.ViewModels.Chat;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EchoBot.Controllers
{
    // This ASP Controller is created to handle a request. Dependency Injection will provide the Adapter and IBot
    // implementation at runtime. Multiple different IBot implementations running at different endpoints can be
    // achieved by specifying a more specific type for the bot constructor argument.
    [Route("api/messages")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly IBot _bot;

        public BotController(IBotFrameworkHttpAdapter adapter, IBot bot)
        {
            _adapter = adapter;
            _bot = bot;
        }



        [HttpGet("/api/messages")]
        public async Task<ActionResult<IEnumerable<BotMessageViewModel>>> GetMessages()
        {
            try
            {
                var messages = GenerateFakeMessages(10);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        public async Task<IActionResult> PostAsync()
        {
            try
            {
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    string requestBody = await reader.ReadToEndAsync();

                    var messageModel = JsonConvert.DeserializeObject<MessageModel>(requestBody);

                    // Process the message model as needed
                    // Example: Send message to bot, process logic, etc.

                    // Example response modification
                    Response.StatusCode = 200;
                    Response.ContentType = "application/json";

                    // Modify response content if needed
                    string modifiedResponseContent = "Modified response content";
                    // Modify the response content
                    string jsonResponse = JsonConvert.SerializeObject(new { text = modifiedResponseContent });
                    await Response.WriteAsync(jsonResponse, Encoding.UTF8);
                }

                // Delegate the processing to the bot adapter (assuming _adapter and _bot are correctly injected)
                //await _adapter.ProcessAsync(Request, Response, _bot);

                var response = new { text = "Modified response content" };

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, "Internal server error"); // Return a 500 Internal Server Error response
            }
        }

        // Method to generate fake messages
        [NonAction]
        public  List<BotMessageViewModel> GenerateFakeMessages(int count)
        {
            List<BotMessageViewModel> messages = new List<BotMessageViewModel>();
            Random random = new Random();

            string[] senders = { "User", "Bot", "Admin" };
            string[] contents = { "Hello!", "How are you?", "What's up?", "Nice to meet you.", "Goodbye!" };

            for (int i = 1; i <= count; i++)
            {
                BotMessageViewModel message = new BotMessageViewModel
                {
              
                    Sender = senders[random.Next(senders.Length)], // Random sender from the senders array
                    Content = contents[random.Next(contents.Length)], // Random content from the contents array
                    SentAt = DateTime.Now.AddDays(-random.Next(1, 30)) // Random date within the last 30 days
                };
                messages.Add(message);
            }

            return messages;
        }

    }
    public class Message
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
    }
}

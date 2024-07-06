using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HMSPortal.Application.AppSettings;
using HMSPortal.Application.Core.Chat.Bot;
using HMSPortal.Application.Core.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static HMSPortal.Application.Core.Chat.Api.IlemaApiResponse;

namespace HMSPortal.Application.Core.Chat.Api
{
    public class IlemaApiRequest
    {
        private readonly HttpClient _client;
        private readonly AppSetting _appSettings;

        public IlemaApiRequest( AppSetting appSettings)
        {
            _appSettings = appSettings;
        }

        public async Task<string> SendMessageAsync(string query)
        {
            var requestData = new
            {

                Key = _appSettings.IlemaKey,
                message = query
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

            try
            {
               

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, _appSettings.Ilemaurl);
                request.Headers.Add("key", _appSettings.IlemaKey);
                request.Headers.Add("Token", _appSettings.IlemaToken);
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
               return responseBody;

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return null;
            }
        }
        public async Task<string> ValidateSyptomsAsync(string query)
        {
            var requestData = new
            {

                Key = _appSettings.IlemaKey,
                message = query
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

            try
            {


                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, _appSettings.Ilemaurl2);
                request.Headers.Add("key", _appSettings.IlemaKey);
                request.Headers.Add("Token", _appSettings.IlemaToken);
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return null;
            }
        }

    }
}

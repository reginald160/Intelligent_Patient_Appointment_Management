using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HMSPortal.Application.AppSettings;
using HMSPortal.Application.Core.Chat.Bot;
using HMSPortal.Application.Core.Chat.Message;
using HMSPortal.Application.Core.Helpers;
using HMSPortal.Application.Core.Response;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static HMSPortal.Application.Core.Chat.Api.IlemaApiResponse;

namespace HMSPortal.Application.Core.Chat.Api
{
    public class LLMApiRequest
    {
        private readonly HttpClient _client;
        private readonly AppSetting _appSettings;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private string rootPath { get; set; }
        public static string ParentPath = "Statics\\SystemContent";
        public LLMApiRequest(AppSetting appSettings, IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _appSettings = appSettings;
            _configuration=configuration;
            _hostingEnvironment=hostingEnvironment;
            rootPath = _hostingEnvironment.ContentRootPath;
        }
        private HttpRequestMessage PrepareChatPostRequest(string requestId, string url)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("key", _appSettings.IlemaKey);
            request.Headers.Add("Token", _appSettings.IlemaToken);
            request.Headers.Add("Request-ID", requestId);
            return request;
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
        public async Task<string> ValidateHealthConditionAsync(string query, string requestId )
        {
            var path = Path.Combine(rootPath, ParentPath, "HealthConditonFilter.txt");
            var system_Content = FileHelper.ReadFileContent(path);
            requestId = DateTime.Now.Ticks.ToString();
            var requestData = new
            {
                inputText = query,
                system_content = system_Content
    
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

            try
            {
                var symptonFilterUrl = _configuration["ChatBot:SymptonFilterUrl"];
                var groupId = _configuration["ChatBot:GroupId"];
                var client = new HttpClient();
                var request = PrepareChatPostRequest(requestId, symptonFilterUrl);
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

		public async Task<string> GroupeDepartmentAsync(string query, string requestId)
		{
			var path = Path.Combine(rootPath, ParentPath, "HealthConditonFilter.txt");
			var system_Content = FileHelper.ReadFileContent(path);
			requestId = DateTime.Now.Ticks.ToString();
			var requestData = new
			{
				inputText = query,
				system_content = system_Content

			};

			var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

			try
			{
				var symptonFilterUrl = _configuration["ChatBot:SymptonFilterUrl"];
				var groupId = _configuration["ChatBot:GroupId"];
				var client = new HttpClient();
				var request = PrepareChatPostRequest(requestId, symptonFilterUrl);
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

		public async Task<string> RequestSymptomAsync(string query, string requestId)
        {
            var path = Path.Combine(rootPath, ParentPath, "HealthConditonFilter.txt");
            var system_Content = FileHelper.ReadFileContent(path);

            var requestData = new
            {
                symptom_description = query

            };

            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

            try
            {
                var symptonFilterUrl = _configuration["ChatBot:SymptomRequest"];
                var client = new HttpClient();
                var request = PrepareChatPostRequest(requestId + "E", symptonFilterUrl);
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                var responseMessage = JsonConvert.DeserializeObject<IlemaApiResponse>(responseBody);
                return responseBody;

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return null;
            }
        }

        public async Task<string> AnalyseHealthCondition(string questions, string healthCondition,  string requestId)
        {
            try

            {
                var requestData = new
                {
                    questions_and_answers = questions,
                    health_condition = healthCondition

                };

                var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

                var symptonAnlyseUrlrl = _configuration["ChatBot:SymptonAnlyseUrl"];
                var client = new HttpClient();
                var request = PrepareChatPostRequest(requestId + "F", symptonAnlyseUrlrl);
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;

          



            }
            catch (Exception exp) {
                return null;
            
            }
        }



    }
}

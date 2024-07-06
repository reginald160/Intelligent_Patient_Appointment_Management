using HMSPortal.Application.AppServices.IServices;
using HMSPortal.Application.Core.Cache;
using HMSPortal.Application.Core.Chat.Bot;
using HMSPortal.Application.Core.MessageBrocker.KafkaBus;
using HMSPortal.Application.ViewModels.Chat;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.Chat.SignalR
{
    public class ChatHub : Hub
    {
        private readonly ResponseModerator _responseModerator;

        public ChatHub(ResponseModerator responseModerator)
        {
            _responseModerator=responseModerator;
 
        }

        public async Task SendMessage(string user, string message)
        {
            //var botResponse =  await _responseModerator.BookAppointmentAsyncFake(message, user);
           var botResponse =  await _responseModerator.ReadMessageAsync(message, user);

            await Clients.All.SendAsync(botResponse.Endpoint, user, botResponse.Message);
            //await Clients.All.SendAsync("ShowDatePicker", user, message);
            //List<string> stringList = new List<string>();

            //// Add some sample strings to the list
            //stringList.Add("Apple");
            //stringList.Add("Banana");
            //stringList.Add("Cherry");
            //stringList.Add("Date");

            //await Clients.All.SendAsync("ReceiveDropDown", user, stringList);


        }
        public async Task SendDate(string user, string message)
        {
            var resp =  await _responseModerator.GetAvaialbleSlotsAsync(message, user);
            if(resp.ResponseType == ResponseType.DropDown)
            {
                await Clients.All.SendAsync(resp.Endpoint, user, resp.Messages);

            }
            else
            {
                await Clients.All.SendAsync(resp.Endpoint, user, resp.Message);
            }

            // await Clients.All.SendAsync(botResponse.Endpoint, user, botResponse.Message);


        }
        public async Task SendDescription(string user, string message)
        {
            var botResponse =  await _responseModerator.ReadMessageAsync(message, user);

            // await Clients.All.SendAsync(botResponse.Endpoint, user, botResponse.Message);
            await Clients.All.SendAsync("ShowDatePicker", user, message);


        }
        public async Task BookAppointment(string user, string message)
        {
            var botResponse =  await _responseModerator.BookAppointmentAsync(message, user);

            // await Clients.All.SendAsync(botResponse.Endpoint, user, botResponse.Message);
            await Clients.All.SendAsync("ShowDatePicker", user, message);


        }
    }
}

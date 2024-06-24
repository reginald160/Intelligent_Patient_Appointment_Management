using HMSPortal.Application.AppServices.IServices;
using HMSPortal.Application.Core.Cache;
using HMSPortal.Application.ViewModels.Chat;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.Chat.SignalR
{
    public class ChatHub : Hub
    {
        private readonly IAppointmentServices _appointmentServices;

        public ChatHub(IAppointmentServices appointmentServices)
        {
            _appointmentServices=appointmentServices;
        }

        public async Task SendMessage(string user, string message)
        {
            
            var receievdChat = new BotMessageViewModel
            {
                Content = message,
                UserId = user,
                Type = CoreValiables.ChatRecieved,

            };
            await _appointmentServices.SaveChat(receievdChat);

            var SentChat = new BotMessageViewModel
            {
                Content = "Response from Bot",
                Type = CoreValiables.ChatSent,
                UserId=user,
            };
            await _appointmentServices.SaveChat(SentChat);
            await Clients.All.SendAsync("ReceiveMessage", user, "Reciceved ddddd");
        }
    }
}

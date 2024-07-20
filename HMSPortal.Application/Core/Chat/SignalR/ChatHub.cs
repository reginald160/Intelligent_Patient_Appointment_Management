using Azure;
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

        public async Task SendGreeting(string user, string message)
        {
            var response = new ChatResponse();
            if (message == "WelcomeMessage")
            {    response = await _responseModerator.GetGreeing(message, user);
                await Clients.All.SendAsync(response.Endpoint, "Bot", response.Message);

            }
            else
            {
                await Clients.All.SendAsync(response.Endpoint, "Bot", response);
            }


        }
        public async Task SendMessage(string user, string message)
        {
            var menu = "Main Menu:\n1. Schedule\n2. Cancel\n3. Reschedule\n4. Exit";

            List<string> stringList = new List<string>();

            // Add some sample strings to the list
            stringList.Add("Schedule");
            stringList.Add("Cancel");
            stringList.Add("Reschedule");
            stringList.Add("Exit");
            var response = GetMenu(message);

            await Clients.All.SendAsync("ReceiveMenu", "Bot", stringList);
        }

        public async Task ReadMenu(string user, string message)
        {
           if(message == "Menu")
            {

                List<string> stringList = new List<string>();
                stringList.Add("Schedule");
                stringList.Add("Cancel");
                stringList.Add("Reschedule");
                stringList.Add("Exit");
            

                await Clients.All.SendAsync("ReceiveMenu", "Bot", stringList);
            }
            else
            {
                var response = GetMenu(message);
                await Clients.All.SendAsync("ReceiveMessage", "Bot", response);


            }

        }
        public async Task SendMessageM(string user, string message)
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


        //public async Task<string> HandleMessage(string user, string message)
        //{
        //    // Determine which question to ask next based on the state
        //    if (!_userResponses.ContainsKey(user))
        //    {
        //        _userResponses[user] = new Dictionary<string, string>();
        //        return "What is the general reason for your appointment? (e.g., Consultation, Follow-up, Routine Check-up, etc.)";
        //    }

        //    var userState = _userResponses[user];

        //    if (!userState.ContainsKey("generalReason"))
        //    {
        //        userState["generalReason"] = message;
        //        return "Can you please describe any specific symptoms or concerns you have been experiencing?";
        //    }

        //    if (!userState.ContainsKey("symptoms"))
        //    {
        //        userState["symptoms"] = message;
        //        return "How long have you been experiencing these symptoms or concerns, and how severe are they on a scale from 1 to 10?";
        //    }

        //    if (!userState.ContainsKey("durationAndSeverity"))
        //    {
        //        userState["durationAndSeverity"] = message;
        //        return "Thank you for the information. We have recorded your reason for the appointment.";
        //    }

        //    // Further processing...
        //    return "How can I assist you further?";
        //}

        public string GetMenu(string message)
        {
            switch (message)
            {
                case "Schedule":
                    return "Schedule an Appointment:\nPlease provide Department/Doctor and Preferred Date/Time.";
                case "Cancel":
                    return "Cancel an Appointment:\nPlease provide Appointment ID or Patient ID.";
                case "Reschedule":
                    return "Reschedule an Appointment:\nPlease provide Appointment ID or Patient ID.";
                case "Exit":
                    return "Thank you for using our service. Goodbye!";
                default:
                    return "Invalid option. Please choose from the main menu.";
            }
        }
    }
}

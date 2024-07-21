using Azure;
using HMSPortal.Application.AppServices.IServices;
using HMSPortal.Application.Core.Cache;
using HMSPortal.Application.Core.Chat.Bot;
using HMSPortal.Application.Core.Chat.Message;
using HMSPortal.Application.Core.MessageBrocker.KafkaBus;
using HMSPortal.Application.ViewModels.Chat;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.Chat.SignalR
{
    public class ChatHub : Hub
    {

        private readonly ResponseModerator _responseModerator;
        private static readonly ConcurrentDictionary<string, ChatTempData> UserConnections = new ConcurrentDictionary<string,ChatTempData>();
        public ChatHub(ResponseModerator responseModerator)
        {
            _responseModerator=responseModerator;
 
        }
        public static void AddOrUpdateUserConnection(string userId, string connectionId)
        {
            var userConnection = new ChatTempData { UserId = userId, ConnectionId = connectionId };
            UserConnections.AddOrUpdate(userId, userConnection, (key, oldValue) => userConnection);
        }
        public override Task OnConnectedAsync()
        {
            var userId = Context.User.GetUserId();
            if (userId != null)
            {
                // Store the connection ID associated with the user ID
                AddOrUpdateUserConnection(userId, Context.ConnectionId);
               
            }

            return base.OnConnectedAsync();
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

            List<string> menu = new List<string> { "Schedule", "Cancel" , "Reschedule", "Exit" };

            //var response = GetMenu(message);

            await Clients.All.SendAsync("ReceiveMenu", "Bot", menu);
        }
        public async Task SendSheduleCategory(string user, string message)
        {
            if (UserConnections.TryGetValue(user, out ChatTempData chatTempData))
            {
                chatTempData.ScheduleType = message;
                // Alternatively, if you want to add to existing questions:
                // chatTempData.Questions.AddRange(newQuestions);

                // Update the dictionary entry to reflect the changes
                UserConnections[user] = chatTempData;
            }
            if (message == "Check-ups")
            {
                
                await Clients.All.SendAsync("ReceiveMessage", "Bot", "Please briefly describe the reason for your check-up, such as general wellness, routine monitoring, or follow-up on a previous condition");

            }
            else
            {
                await Clients.All.SendAsync("ReceiveMessage", "Bot", "Please describe the symptoms or health issues you are experiencing.\nInclude any relevant details such as the onset of symptoms, their severity, and how they have been affecting your daily life");

            }
        }
        public async Task ValidateHealthCondition(string user, string message)
        {
            var respo = await _responseModerator.ValideHealthCondition(message, user);
            if(respo != null && respo.Messages.Any()) 
            {
                if (UserConnections.TryGetValue(user, out ChatTempData chatTempData))
                {
                    chatTempData.HealthCondition = message;
                    chatTempData.Questions = respo.Messages;
                    // Alternatively, if you want to add to existing questions:
                    // chatTempData.Questions.AddRange(newQuestions);

                    // Update the dictionary entry to reflect the changes
                    UserConnections[user] = chatTempData;
                }
                await Clients.All.SendAsync("ReceiveQuestions", "Bot", respo.Messages.ToList());
            }
            else
            {
                //await Clients.All.SendAsync(respo.Endpoint, "Bot", respo.Message);
                var questions = new List<string> { "What is your name?", "How old are you?", "What is your email address?" };
                //await Clients.Client(user).SendAsync("ReceiveQuestions", "Bot", questions);
                await Clients.All.SendAsync(respo.Endpoint, "Bot", respo.Messages);

            }

        }

        public async Task SubmitQuestions(string userId, string answersJson)
        {
            if (UserConnections.TryGetValue(userId, out ChatTempData chatTempData))
            {

                chatTempData.QuestionsAndAnswers = answersJson;
                UserConnections[userId] = chatTempData;
            }
            var response = await _responseModerator.AnalyseSymmtonFeedback(answersJson, userId, chatTempData.HealthCondition);
            if(response.validation_status.Contains("VALID"))
            {
                chatTempData.Result = response.message;
                UserConnections[userId] = chatTempData;
                var mesg = "Please select a date suitable for your appointment;";
                await Clients.All.SendAsync("ShowDatePicker", "Bot", mesg);

                
            }
            else
            {
                var mesg = "Please select a date suitable for your appointment\";\r\n";
                await Clients.All.SendAsync("ShowDatePicker", "Bot", mesg);

            }
            // Process the answers as needed
        }

        public async Task SendSymtoms(string user, string message)
        {
            var response = await _responseModerator.ValideHealthCondition(user, message);
            await Clients.All.SendAsync("ReceiveMessage", "Bot", "Please describe the symptoms or health issues you are experiencing.\nInclude any relevant details such as the onset of symptoms, their severity, and how they have been affecting your daily life");

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
           else if(message.Contains("Check-ups") || message.Contains("New Health Concerns"))
            {
                if (UserConnections.TryGetValue(user, out ChatTempData chatTempData))
                {
                    chatTempData.ScheduleType = message;
                    // Alternatively, if you want to add to existing questions:
                    // chatTempData.Questions.AddRange(newQuestions);

                    // Update the dictionary entry to reflect the changes
                    UserConnections[user] = chatTempData;
                }

                if (message == "Check-ups")
                {

                    await Clients.All.SendAsync("ReceieveSheduleCategory", "Bot", "Please briefly describe the reason for your check-up, such as general wellness, routine monitoring, or follow-up on a previous condition");


                }
                else
                {
                    await Clients.All.SendAsync("ReceieveSheduleCategory", "Bot", "Please describe the symptoms or health issues you are experiencing.\nInclude any relevant details such as the onset of symptoms, their severity, and how they have been affecting your daily life");

                }
            }
            else
            {
                var response = GetMenu(message);
                await Clients.All.SendAsync("ReceiveMenuMessage", "Bot", response);


            }

        }
      
        public async Task SendDate(string user, string message)
        {
            var resp =  await _responseModerator.GetAvaialbleSlotsAsync(message, user);
            if(resp.ResponseType == ResponseType.DropDown)
            {
                var date = DateTime.Parse(message);
                if (UserConnections.TryGetValue(user, out ChatTempData chatTempData))
                {

                    chatTempData.Date = date;
                    UserConnections[user] = chatTempData;
                }
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

        public string GetMenu(string message)
        {
            switch (message)
            {
                case "Schedule":
                    return "Schedule an Appointment:\nPlease select appointment category";
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

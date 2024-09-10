using DHTMLX.Scheduler;
using HMSPortal.Application.AppServices.IServices;
using HMSPortal.Application.Core.Chat.Api;
using HMSPortal.Application.Core.Chat.Message;
using HMSPortal.Application.Core.Helpers;
using HMSPortal.Application.Core.MessageBrocker.EmmaBrocker;
using HMSPortal.Application.Core.MessageBrocker.KafkaBus;
using HMSPortal.Application.ViewModels.Appointment;
using HMSPortal.Application.ViewModels.Chat;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.Chat.Bot
{

    public class ResponseModerator
    {
        private readonly IAppointmentServices _appointmentServices;
        private readonly LLMApiRequest _ilema;
        private readonly IPatientServices _petientServices;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IMessageBroker _messageBroker;
        private static readonly ConcurrentDictionary<string, ChatTempData> UserConnections = new ConcurrentDictionary<string, ChatTempData>();

        public ResponseModerator(IAppointmentServices appointmentServices, LLMApiRequest ilema, IMessageBroker messageBroker, IPatientServices petientServices, IHttpContextAccessor httpContextAccessor)
        {
            _appointmentServices=appointmentServices;
            _ilema=ilema;
            _messageBroker=messageBroker;
            _petientServices=petientServices;
            _httpContextAccessor=httpContextAccessor;
            
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            AddOrUpdateUserConnection(userId, userId);
        }

        private static ChatTempData GetUserConnection(string userId)
        {
            UserConnections.TryGetValue(userId, out var userConnection);
            return userConnection;
        }
        public static void AddOrUpdateUserConnection(string userId, string connectionId)
        {
            var userConnection = new ChatTempData { UserId = userId, ConnectionId = connectionId };
            UserConnections.AddOrUpdate(userId, userConnection, (key, oldValue) => userConnection);
        }
		public async Task Savemessage(BotMessageViewModel message)
        {
			
			await _appointmentServices.SaveChat(message);
		}


		public async Task<ChatResponse> GetGreeing(string message, string UserId)
        {

            try
            {
                AddOrUpdateUserConnection(UserId, Guid.NewGuid().ToString());
                var patient = _petientServices.GetPatientById(Guid.Parse(UserId));
                var greeting = ChatHelper.SayGreeting(patient.FirstName);
                var chatResponse = new ChatResponse
                {
                    Endpoint = CoreValiables.ChatTextEndpoint,

                };
                var receievdChat = new BotMessageViewModel
                {
                    Content = greeting,
                    UserId = UserId,
                    Type = CoreValiables.ChatSent,
                    HasOptions = false,

                };
                await _appointmentServices.SaveChat(receievdChat);

                return new ChatResponse { Message = greeting};
            }
            catch (Exception ex)
            {
                return new ChatResponse();
            }
        }

        public async Task<ChatResponse> RescheduleAppointment(string UserId, ChatTempData data)
        {
            var model = new AddAppointmentViewModel
            {
                TimeSlot = data.Slot,
                Date = data.Date,
                PatientId = UserId

            };
           await  _appointmentServices.RescheduleAppointmentByPatient(model, data.AppointmentId);
			var msg = "Your appointment has been succesfully rescheduled";
			var sentChat = new BotMessageViewModel
			{
				Content = msg,
				UserId = UserId,
				Type = CoreValiables.ChatSent,
				HasOptions = false,

			};

			return new ChatResponse
            {
                Message = msg,
                Endpoint = "ReceiveSuccessMessage"
			};
        }
        public async Task<ChatResponse> ReadMessageAsync(string message, string UserId)
        {
            try
            {
                var chatResponse = new ChatResponse
                {
                    Endpoint = CoreValiables.ChatTextEndpoint,

                };
                var receievdChat = new BotMessageViewModel
                {
                    Content = message,
                    UserId = UserId,
                    Type = CoreValiables.ChatRecieved,

                };
                await _appointmentServices.SaveChat(receievdChat);

                var responseBody = await _ilema.SendMessageAsync(message);
                var response = JsonConvert.DeserializeObject<IlemaApiResponse>(responseBody);
                var cleansMessage = ExtractIntentAndCleanMessage(response.Message);
                string chatContent = "";
                chatResponse.Message = cleansMessage.cleanedMessage;

                if (cleansMessage.Intent == "BOOK_APPOINTMENT" || cleansMessage.Intent == "READY_APPOINTMENT")
                {
                    chatResponse.Message = "I can help you schedule a new appointment. please select a date suitable for your appointment";
                    chatResponse.Endpoint = "ReceiveDescription";
                }
                else
                {
                    if (chatResponse.Message.Contains("BOOK_APPOINTMENT", StringComparison.OrdinalIgnoreCase))
                    {
                        chatResponse.Message =  chatResponse.Message.Replace("BOOK_APPOINTMENT", "book appointment", StringComparison.OrdinalIgnoreCase);
                    }
                }
                //var options = new List<string> { "Option 1", "Option 2", "Option 3" };

                //var optionObject = JsonConvert.SerializeObject(options);
                var SentChat = new BotMessageViewModel
                {
                    Content = "Response from Bot",
                    Type = CoreValiables.ChatSent,
                    UserId=UserId,
                    HasOptions = false,
                    Options = "sss",
                    APIResponse = responseBody,
                    BotMessage = cleansMessage.cleanedMessage,
                    UserIntent = cleansMessage.Intent

                };

                await _appointmentServices.SaveChat(SentChat);

                return chatResponse;
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        public async Task<HealtResponse> AnalyseSymmtonFeedback(string message, string UserId, string healthCondition)
        {
            try
            {
                var answers = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);

                var query = FormatQuestionsAndAnswers(answers);

                var chatResponse = new ChatResponse
                {
                    Endpoint = CoreValiables.ChatTextEndpoint,

                };
                //var receievdChat = new BotMessageViewModel
                //{
                //    Content = message,
                //    UserId = UserId,
                //    Type = CoreValiables.ChatRecieved,

                //};
                //await _appointmentServices.SaveChat(receievdChat);

                var responseBody = await _ilema.AnalyseHealthCondition(message, UserId, healthCondition);
                var response = JsonConvert.DeserializeObject<HealtResponse>(responseBody);
                string chatContent = "";
                

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        public async Task<ChatResponse> CancelAppointmentAsync(string appointmentId, string userId)
        {
            try
            {
                var resp = await _appointmentServices.CancelAppointment(userId,appointmentId);
                return new ChatResponse
                {
                    Message = "Your appointment has been cancelled succesfully",
                    Endpoint = "ReceiveSuccessMessage"

                };
            }
            
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<ChatResponse> ReadRescheduleAsync(string message, string userId)
        {
            try
            {
                var resp = await _appointmentServices.GetRecentAppointmentByPatient(userId);
                var appointments = resp.Data as List<AllAppointmentViewModel>;
                appointments = appointments.Where(x => x.Status == "Up coming" || x.Status == "UpComming").ToList();
                if (appointments == null || !appointments.Any())
                {
                    return new ChatResponse
                    {
                        Message = "Your currently do not have an upcoming appointment,\n kindly go proceed to scheduling a new appointment",
                        Endpoint = "ReceiveReschedule"

                    };
                }
                else
                {
                    var responses = appointments.Select(x => x.ReferenceNumber).ToList();
                    return new ChatResponse
                    {
                        //Message = "Please kindly scheduling select a date",
                        Message = "RSC",
                        Messages = responses,
                        Endpoint = "ReceiveReschedule"

                    };
                }
            }
            catch(Exception ex)
            {
                return null;
            }

        }
        public async Task<List<string>> GetAppointmentByUser(string message, string userId)
        {
            var resp = await _appointmentServices.GetRecentAppointmentByPatient(userId);
            var appointments = resp.Data as List<AllAppointmentViewModel>;
            return appointments.Select(x => x.ReferenceNumber).ToList();
        }
        public async Task<ChatResponse> ReadMenuAsync(string message, string userId)
        {
            try
            {
                var chatMessage = message;
                message = GetMenu(message);
                var response = new ChatResponse
                {
                    Message = message,
                    Endpoint = "ReceiveMenuMessage"

                };
             
                if (message.Contains("Reschedule"))
                {
                    var resp = await _appointmentServices.GetRecentAppointmentByPatient(userId);
                    var appointments = resp.Data as List<AllAppointmentViewModel>;
                    appointments = appointments.Where(x=> x.Status == "Up coming" || x.Status == "UpComming").ToList();
                    if(appointments == null || !appointments.Any())
                    {
                        return new ChatResponse
                        {
                            Endpoint = "ReceiveRescheduleDefault",
                            Message = "Your currently do not have an upcoming appointment,\n kindly go proceed to scheduling a new appointment\";"
                        };
                        
                    }
                    else
                    {
                        return new ChatResponse
                        {
                            Endpoint = "ReceiveMenuMessage",
                            Message = message,
                        };
                        
                    }
                    
                }
                if(message.Contains("Cancel an Appointment"))
                {
                    var resp = await _appointmentServices.GetRecentAppointmentByPatient(userId);
                    var appointments = resp.Data as List<AllAppointmentViewModel>;
                    appointments = appointments.Where(x => x.Status == "Up coming" || x.Status == "UpComming").ToList();
                    if (appointments == null || !appointments.Any())
                    {
                        return new ChatResponse
                        {
                            Endpoint = "ReceiveRescheduleDefault",
                            Message = "Your currently do not have an upcoming appointment,\n kindly go proceed to scheduling a new appointment\";"
                        };

                    }
                    else
                    {
                        return new ChatResponse
                        {
                            Endpoint = "ReceiveMenuMessage",
                            Message = message,
                        };

                    }
                }
				var receievdChat = new BotMessageViewModel
				{
					Content = chatMessage,
					UserId = userId,
					Type = CoreValiables.ChatRecieved,
					HasOptions = false,

				};
				await _appointmentServices.SaveChat(receievdChat);

				var sentChat = new BotMessageViewModel
				{
					Content = response.Message,
					UserId = userId,
					Type = CoreValiables.ChatSent,
					HasOptions = false,

				};
				await _appointmentServices.SaveChat(sentChat);
                if (message.Contains("Schedule"))
                {				
					List<string> menu = new List<string> { "Check-ups", "New Health Concerns" };
					var schedulePotions = new BotMessageViewModel
					{
						Content = message,
						UserId = userId,
						Type = CoreValiables.ChatSent,
						HasOptions = true,
						Options = JsonConvert.SerializeObject(menu)

					};
					await _appointmentServices.SaveChat(schedulePotions);
				}
				//Log user and Bot
				return response;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        public async Task<ChatResponse> ValideHealthCondition(string message, string UserId)
        {
            try
            {
                var chatResponse = new ChatResponse
                {
                    Endpoint = CoreValiables.ChatTextEndpoint,
                    
                };
				var receievdChat = new BotMessageViewModel
				{
					Content = message,
					UserId = UserId,
					Type = CoreValiables.ChatRecieved,
					HasOptions = false,

				};
				await _appointmentServices.SaveChat(receievdChat);

				var sentdChat = new BotMessageViewModel
				{
					Content = message,
					UserId = UserId,
					Type = CoreValiables.ChatSent,
					HasOptions = false,

				};
				
				//var receievdChat = new BotMessageViewModel
				//{
				//    Content = message,
				//    UserId = UserId,
				//    Type = CoreValiables.ChatRecieved,

				//};
				//await _appointmentServices.SaveChat(receievdChat);

				var responseBody = await _ilema.ValidateHealthConditionAsync(message, UserId);
                var response = JsonConvert.DeserializeObject<IlemaApiResponse>(responseBody);
                string chatContent = "";                
                if (response.Message.Contains("INVALID") || response.Message.Contains("OFFPOINT"))
                {
                    chatResponse.Message = $" '{message}' does not relate to any health description or condition" +
                        $"\n Please describe the symptoms or health issues you are experiencing. \nInclude any relevant details such as the onset of symptoms, \n their severity, and how they have been affecting your daily life";
                    
                    chatResponse.Endpoint = "ReceieveSheduleCategory";
                }
                else if (response.Message.Contains("VALID")){

                    var symptonResponse = await _ilema.RequestSymptomAsync(message, DateTime.Now.Ticks.ToString());
                    var responseMessage = JsonConvert.DeserializeObject<IlemaApiResponse>(symptonResponse);
                    if (responseMessage.Message.Contains("VALID"))
                    {
                       var questions =  Logics.ExtractQuestions(responseMessage.Message);
                        chatResponse.Endpoint = "ValidateMessage";
                        chatResponse.Messages = questions;
                        chatResponse.Endpoint = "ReceiveQuestions";
                        sentdChat.HasOptions = true;
                        sentdChat.Options = string.Concat("\n ,", questions);
                    }

                }
                else 
                {
                    chatResponse.Message = message + " does not relate to any health description or condition";
                    chatResponse.Endpoint = "SendSymtoms";
                    chatResponse.Endpoint = "SendSymtoms";
                }
                sentdChat.Content = chatResponse.Message;
				await _appointmentServices.SaveChat(sentdChat);
				return chatResponse;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<ChatResponse> GetAvaialbleSlotsAsync(string message, string UserId)
        {
			var receievdChat = new BotMessageViewModel
			{
				Content = message,
				UserId = UserId,
				Type = CoreValiables.ChatRecieved,
				HasOptions = false,

			};
			await _appointmentServices.SaveChat(receievdChat);

			if (string.IsNullOrEmpty(message))
            {
                var msg = $"invalid date selected  {message}," +
                $" Please choose an alternative date for your appointment ";
				var sentChat = new BotMessageViewModel
				{
					Content = msg,
					UserId = UserId,
					Type = CoreValiables.ChatSent,
					HasOptions = false,

				};
				await _appointmentServices.SaveChat(sentChat);
				return new ChatResponse
                {
                    Endpoint = "ShowDatePicker",
                    Message = msg,
                };

            }

            var date = DateTime.Parse(message);
            var slots = _appointmentServices.GetAvailableSlotsForDateToString(date);
            if (slots.Count < 1)
            {
                var msg = $"there are no available slots for your selected date {message}," +
                $" Please choose an alternative date for your appointment ";
				var sentChat = new BotMessageViewModel
				{
					Content = msg,
					UserId = UserId,
					Type = CoreValiables.ChatSent,
					HasOptions = false,

				};
				await _appointmentServices.SaveChat(sentChat);
				return new ChatResponse
                {
                    Endpoint = "ShowDatePicker",
                    Message = msg,
                };
            }
            else
            {
                var msg = "please select a  suitable date for your appointment";
				var sentChat = new BotMessageViewModel
				{
					Content = msg,
					UserId = UserId,
					Type = CoreValiables.ChatSent,
					HasOptions = false,

				};
				await _appointmentServices.SaveChat(sentChat);
				return new ChatResponse
                {
                    Endpoint = "ReceiveDropDown",
                    Message = "",
                    ResponseType = ResponseType.DropDown,
                    Messages =  slots
                };
            }
        }
    
        private MessageDescriptionResponse ExtractDescriptionAndCleanMessage(string response)
        {
            var message = "I'm sorry, the given health description is not provided in the message. Could you please provide the necessary information for analysis? Without sufficient symptoms or context, it's impossible to determine the appropriate department for treatment. Please respond with your symptoms for accurate analysis";
            if (response.Contains("NOTENOUGH"))
            {
                // Regular expression to match the intent and the following newlines
                string pattern = @"NOTENOUGH:\s*(\w+)\s*\n+";
                Match match = Regex.Match(response, pattern);

                if (match.Success)
                {
                    string intent = match.Groups[1].Value;
                    string cleanedMessage = Regex.Replace(response, pattern, "");
                    return new MessageDescriptionResponse
                    {
                        NotEnough = cleanedMessage,
                    };


                }
                else
                {
                    // If no intent is found, return the original message
                    return new MessageDescriptionResponse
                    {
                        NotEnough = message,
                    };
                }
            }
            if(response.Contains("SYMPTOM") && response.Contains("DEPARTMENT"))
            {
                var rep = ExtractSymptomAndDepartment(response);
                if(rep != null)
                {
                    return new MessageDescriptionResponse
                    {
                        Department = rep.Department,
                        Sympton = rep.Sympton,
                    };
                }
                else
                {
                    return new MessageDescriptionResponse
                    {
                        NotEnough = message
                    };
                }

            }
            else
            {
                return new MessageDescriptionResponse
                {
                    NotEnough = message
                };
            }



        }
        private  MessageCleanerResponse ExtractIntentAndCleanMessage(string response)
        {
            // Regular expression to match the intent and the following newlines
            string pattern = @"INTENT:\s*(\w+)\s*\n+";

            Match match = Regex.Match(response, pattern);

            if (match.Success)
            {
                string intent = match.Groups[1].Value;
                string cleanedMessage = Regex.Replace(response, pattern, "");
                return new MessageCleanerResponse
                {
                    Intent = intent,
                    cleanedMessage = cleanedMessage
                };


            }
            else
            {
                // If no intent is found, return the original message
                return null;
            }
        }

        public async Task<ChatResponse> BookAppointmentAsync(string message, string UserId, ChatTempData data)
        {
               
            try
            {
                
                var convert = Logics.ExtractDate(message);
                var appointment = new AddAppointmentViewModel
                {
                    Date = convert.Item1,
                    PatientId = UserId,
                    ProblemDescrion = data.Result,
                    TimeSlot =convert.Item2,
                    Department = "Dental",
                    AppointmentType = data.ScheduleType
                };
                var appointmentMessage = JsonConvert.SerializeObject(appointment);
                await _messageBroker.PublishAsync(CoreValiables.BootAppointmentTopic, appointmentMessage);
                var msg = "Your appointment has been booked successfully";
				var sentChat = new BotMessageViewModel
				{
					Content = msg,
					UserId = UserId,
					Type = CoreValiables.ChatSent,
					HasOptions = false,

				};
				await _appointmentServices.SaveChat(sentChat);
				return new ChatResponse
                {
                    Message = msg,
                    Endpoint = "ShowDatePicker"
                };
            }
            catch (Exception ex)
            {
                return new ChatResponse();
            }
        }

        public async Task<ChatResponse> BookAppointmentAsyncFake(string message, string UserId)
        {


            try
            {
                
                var appointment = new AddAppointmentViewModel
                {
                    Date = DateTime.Now,
                    PatientId = message,
                    ProblemDescrion = "sick patinece",
                    TimeSlot ="",
                    Department = "Dental"
                };
                var appointmentMessage = JsonConvert.SerializeObject(appointment);
                await  _messageBroker.PublishAsync(CoreValiables.BootAppointmentTopic, appointmentMessage);
                return new ChatResponse();
            }
            catch (Exception ex)
            {
                return new ChatResponse();
            }
        }
  

        static MessageDescriptionResponse ExtractSymptomAndDepartment(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return null;

            var lines = content.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            string symptom = null;
            string department = null;

            foreach (var line in lines)
            {
                if (line.StartsWith("SYMPTOM:", StringComparison.OrdinalIgnoreCase))
                {
                    symptom = line.Substring("SYMPTOM:".Length).Trim();
                }
                else if (line.StartsWith("DEPARTMENT:", StringComparison.OrdinalIgnoreCase))
                {
                    department = line.Substring("DEPARTMENT:".Length).Trim();
                }

                if (!string.IsNullOrEmpty(symptom) && !string.IsNullOrEmpty(department))
                {
                    return new MessageDescriptionResponse
                    {
                        Sympton = symptom,
                        Department = department
                    };
                }
            }

            return null;
        }

        static string FormatQuestionsAndAnswers(Dictionary<string, string> answers)
        {
            string formattedOutput = "Kindly review all the questions and answers and see if the each question is sufficient for the answer:\n";

            foreach (var entry in answers)
            {
                formattedOutput += $"Question: {entry.Key}\nAnswer: {entry.Value}\n\n";
            }

            return formattedOutput;
        }
         string GetMenu(string message)
        {
            switch (message)
            {
                case "Schedule":
                    return "Schedule an Appointment:\nPlease select appointment category";
                case "Cancel":
                    return "Cancel an Appointment:\nPlease select appointment for cancellation";
                case "Reschedule":
                    return "Reschedule an Appointment:\nPlease select appointment";
                case "Exit":
                    return "Thank you for using our service. Goodbye!";
                default:
                    return "Invalid option. Please choose from the main menu.";
            }
        }


    }

}

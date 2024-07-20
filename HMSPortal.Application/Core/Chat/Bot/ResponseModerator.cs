using DHTMLX.Scheduler;
using HMSPortal.Application.AppServices.IServices;
using HMSPortal.Application.Core.Chat.Api;
using HMSPortal.Application.Core.Chat.Message;
using HMSPortal.Application.Core.Helpers;
using HMSPortal.Application.Core.MessageBrocker.EmmaBrocker;
using HMSPortal.Application.Core.MessageBrocker.KafkaBus;
using HMSPortal.Application.ViewModels.Appointment;
using HMSPortal.Application.ViewModels.Chat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.Chat.Bot
{
    public class ResponseModerator
    {
        private readonly IAppointmentServices _appointmentServices;
        private readonly IlemaApiRequest _ilema;
        private readonly IPatientServices _petientServices;

        private readonly IMessageBroker _messageBroker;

        public ResponseModerator(IAppointmentServices appointmentServices, IlemaApiRequest ilema, IMessageBroker messageBroker, IPatientServices petientServices)
        {
            _appointmentServices=appointmentServices;
            _ilema=ilema;
            _messageBroker=messageBroker;
            _petientServices=petientServices;
        }

        public async Task<ChatResponse> GetGreeing(string message, string UserId)
        {

            try
            {
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
                    Type = CoreValiables.ChatRecieved,

                };
                await _appointmentServices.SaveChat(receievdChat);

                return new ChatResponse { Message = greeting};
            }
            catch (Exception ex)
            {
                return new ChatResponse();
            }
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
            catch(Exception ex)
            {
                throw new Exception();
            }
        }

        public async Task<ChatResponse> GetAvaialbleSlotsAsync(string message, string UserId)
        {
            
            if(string.IsNullOrEmpty(message))
            {
                var msg = $"invalid date selected  {message}," +
                $" Please choose an alternative date for your appointment ";
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
                return new ChatResponse
                {
                    Endpoint = "ShowDatePicker",
                    Message = msg,
                };
            }
            else
            {

                return new ChatResponse
                {
                    Endpoint = "ReceiveDropDown",
                    Message = "please select a  suitable for your appointment",
                    ResponseType = ResponseType.DropDown,
                    Messages =  slots
                };
            }
        }
        //private string CheckStringValue(string value)
        // {
        //     switch (value.ToLower())
        //     {
        //         case "READY_APPOINTMENT":
        //             Console.WriteLine("You selected Apple.");
        //             break;
        //         case "BOOK_APPOINTMENT":
        //             Console.WriteLine("You selected Banana.");
        //             break;
        //         case "cherry":
        //             Console.WriteLine("You selected Cherry.");
        //             break;
        //         case "date":
        //             Console.WriteLine("You selected Date.");
        //             break;
        //         default:
        //             Console.WriteLine("Unknown selection.");
        //             break;
        //     }
        // }
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

        public async Task<ChatResponse> BookAppointmentAsync(string message, string UserId)
        {
          
            
            try
            {
      
                var convert = Logics.ExtractDate(message);
                var appointment = new AddAppointmentViewModel
                {
                    Date = convert.Item1,
                    PatientId = UserId,
                    ProblemDescrion = "sick patinece",
                    TimeSlot =convert.Item2,
                    Department = "Dental"
                };
                var appointmentMessage = JsonConvert.SerializeObject(appointment);
                await _messageBroker.PublishAsync(CoreValiables.BootAppointmentTopic, appointmentMessage);
                return new ChatResponse
                {
                    Message = "I can help",
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
        //public async Task<ChatResponse> ValidateDesription(string message, string UserId)
        //{
        //    var responseBody = await _ilema.SendMessageAsync(message);
        //    var response = JsonConvert.DeserializeObject<IlemaApiResponse>(responseBody);
        //    var cleansMessage = ExtractDescriptionAndCleanMessage(response.Message);
            
        //    if(!string.IsNullOrEmpty(cleansMessage.NotEnough))
        //    {
        //        return new ChatResponse
        //        {
        //            Message = cleansMessage.NotEnough,
        //            Endpoint = "ReceiveDescription"
        //        };
        //    }
        //    if(!string.IsNullOrEmpty(cleansMessage.Department) || !string.IsNullOrEmpty(cleansMessage.Sympton))
        //    {
        //        return new ChatResponse
        //        {
        //            Message = "I can help you schedule a new appointment. please select a date suitable for your appointment",
        //            Endpoint = "ReceiveDescription"
        //        };
        //    }


        //}

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

    }

}

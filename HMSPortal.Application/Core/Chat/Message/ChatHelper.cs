using HMSPortal.Application.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.Chat.Message
{
    public class ChatHelper
    {
		
			private static readonly Random random = new Random();

			public static string SayGreeting(string name)
			{
				List<string> greetings = new List<string>
				{
					$"{Logics.GetGreeting()} {name}! Welcome to MediSmart hospital appointment system.\nHow may I assist you today?",
					$"{Logics.GetGreeting()} {name} {name}! Welcome to MediSmart hospital appointment system.\nHow may I assist you today?",
					$"{Logics.GetGreeting()} {name} {name}! You are now in MediSmart hospital appointment system.\nHow can I help you today?",
					$"{Logics.GetGreeting()} {name}, welcome to MediSmart hospital appointment system.\nWhat can I do for you today?",
					$"{Logics.GetGreeting()} {name} {name}! You have reached MediSmart hospital appointment system.\nHow may I assist you?"
				};

				string selectedGreeting = greetings[random.Next(greetings.Count)];

				return selectedGreeting;
			}
		}

	
}


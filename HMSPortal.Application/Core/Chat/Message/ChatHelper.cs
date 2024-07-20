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
        public static string SayGreeting(string name)
        {
            return $"{Logics.GetGreeting()} {name}! Welcome to MediSmart hospital appointment system.\n How may I assist you today?";
        }

    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core
{
	public static class CoreValiables
	{
		public  const string SequenceNumberFormat = "D4";
		public const string PatientCountCacheKey = "PatientCount";
		public const string LoginUser = "LoginUser";
		public const string CacheUserId = "UserId";
		public const string CacheUserEmail = "UserEmail";
        public const string ChatSent = "Response";
        public const string ChatRecieved = "Request";
        public const string ChatTextEndpoint = "ReceiveMessage";
        public const string ChatOptionsEndpoint = "ReceiveOptions";
        public const string BootAppointmentTopic = "BookAppointment";
    }
}

using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.Chat.SignalR
{
	public class UserHub : Hub
	{
		public override async Task OnConnectedAsync()
		{
			if (Context.User.Identity.IsAuthenticated)
			{
				await Groups.AddToGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
			}
			await base.OnConnectedAsync();
		}
		public async Task ForceLogout(string userId)
		{
			await Clients.User(userId).SendAsync("ForceLogout");
		}
	}
}

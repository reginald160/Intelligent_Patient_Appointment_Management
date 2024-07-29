// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.22.0

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot.Bots
{
    public class EchoBot : IBot //ActivityHandler
    {
        //protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        //{
        //    if(turnContext.Activity.Attachments is null)
        //    {
        //        var replyText = $"Echo: {turnContext.Activity.Text}";
        //        await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
        //    }
        //    else
        //    {
        //        var suggestedActions = new List<Attachment>();
        //        suggestedActions.AddRange(turnContext.Activity.Attachments);


        //        //await turnContext.SendActivityAsync(MessageFactory.ContentUrl("https://res.cloudinary.com/dukd0jnep/image/upload/v1707150014/mosov2hk8yb4nkmalwnv.png", "message url"));
        //        //await turnContext.SendActivityAsync(MessageFactory.SuggestedActions(suggestedActions, "Choose an option:", "Optional SSML content", "expectingInput"));
        //        //await turnContext.SendActivityAsync(MessageFactory.Carousel(suggestedActions, "Choose an option:", "Optional SSML content", "expectingInput"));

        //        await turnContext.DeleteActivityAsync(turnContext.Activity.Id, cancellationToken);
        //    }
        //    //var replyText = $"Echo: {turnContext.Activity.Text}";
        //    //await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
        //}

        //protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        //{
        //    var welcomeText = "Hello and welcome!";
        //    foreach (var member in membersAdded)
        //    {
        //        if (member.Id != turnContext.Activity.Recipient.Id)
        //        {
        //            await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
        //        }
        //    }
        //}

        //protected override async Task OnMessageDeleteActivityAsync(ITurnContext<IMessageDeleteActivity> turnContext, CancellationToken cancellationToken)
        //{
        //    await turnContext.SendActivityAsync(MessageFactory.Text("ssss", "ssss"), cancellationToken);
        //    //return base.OnMessageDeleteActivityAsync(turnContext, cancellationToken);
        //}
        //}

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                var userMessage = turnContext.Activity.Text;
                var replyText = $"You said: {userMessage}";

                // Save message to database
                //var chatMessage = new ChatMessage
                //{
                //    UserId = turnContext.Activity.From.Id,
                //    Message = userMessage,
                //    Response = replyText,
                //    Timestamp = DateTime.UtcNow
                //};

                //_context.ChatMessages.Add(chatMessage);
                //await _context.SaveChangesAsync();

                // Indicate typing
                await turnContext.SendActivitiesAsync(new IActivity[]
                {
                new Activity { Type = ActivityTypes.Typing },
                new Activity { Type = ActivityTypes.Delay, Value = 1000 } // Simulate typing delay
                }, cancellationToken);

                await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
            }
        }
    }
}

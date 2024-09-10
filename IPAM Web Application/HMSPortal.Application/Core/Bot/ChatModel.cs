using System;

namespace EchoBot.Model
{
    public class MessageModel
    {
        public string Text { get; set; }
        public string TextFormat { get; set; }
        public string Type { get; set; }
        public ChannelData ChannelData { get; set; }
        public string ChannelId { get; set; }
        public FromModel From { get; set; }
        public string Locale { get; set; }
        public DateTime Timestamp { get; set; }
        public ConversationModel Conversation { get; set; }
        public string Id { get; set; }
        public DateTime LocalTimestamp { get; set; }
        public RecipientModel Recipient { get; set; }
        public string ServiceUrl { get; set; }
    }

    public class ChannelData
    {
        public string ClientActivityID { get; set; }
    }

    public class FromModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
    }

    public class ConversationModel
    {
        public string Id { get; set; }
    }

    public class RecipientModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
    }

}

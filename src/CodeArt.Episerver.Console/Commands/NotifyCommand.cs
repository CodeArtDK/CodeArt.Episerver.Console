using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using EPiServer.Notification;
using EPiServer.Personalization;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command(Keyword = "notify", Description ="Sends a notification to an editor")]
    public class NotifyCommand : IConsoleCommand, IInputCommand
    {
        [CommandParameter]
        public string ChannelName { get; set; }
        [CommandParameter]
        public string NotificationType { get; set; }
        [CommandParameter]
        public string Sender { get; set; }
        [CommandParameter]
        public string Recipient { get; set; }
        [CommandParameter]
        public string Subject { get; set; }
        [CommandParameter]
        public string Content { get; set; }

        private readonly INotifier _notifier;

        public NotifyCommand(INotifier notifier)
        {
            _notifier = notifier;
        }


        public string Execute(params string[] parameters)
        {
            if (ChannelName == null) ChannelName = "DeveloperInfo";
            if (NotificationType == null) NotificationType = "Info";
            if (Sender == null) Sender = EPiServerProfile.Current.UserName;
            if (Recipient == null) Recipient = EPiServerProfile.Current.UserName;
            if (Subject == null) return "Unable to send notification without Subject";
            if (Content == null) Content = "";
            var message = new NotificationMessage
            {
                ChannelName = ChannelName,
                TypeName = NotificationType,
                Sender = new NotificationUser(Sender), //EPiServerProfile.Current.UserName
                Recipients = new[]
                {
                    new NotificationUser(Recipient)
                },
                Subject = Subject,
                Content = Content
            };
            _notifier.PostNotificationAsync(message).Wait();
            return "User notified";
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            Source.OnCommandOutput += Source_OnCommandOutput;
        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            if(output is IDictionary<string, object>)
            {
                var dic = output as IDictionary<string, object>;
                if (ChannelName == null && dic.ContainsKey("ChannelName")) ChannelName = dic["ChannelName"].ToString();
                if (NotificationType == null && dic.ContainsKey("NotificationType")) ChannelName = dic["NotificationType"].ToString();
                if (Sender == null && dic.ContainsKey("Sender")) ChannelName = dic["Sender"].ToString();
                if (Recipient == null && dic.ContainsKey("Recipient")) ChannelName = dic["Recipient"].ToString();
                if (Subject == null && dic.ContainsKey("Subject")) ChannelName = dic["Subject"].ToString();
                if (Content == null && dic.ContainsKey("Content")) ChannelName = dic["Content"].ToString();


            }
            else if (Content != null) Content = output.ToString();
        }
    }
}

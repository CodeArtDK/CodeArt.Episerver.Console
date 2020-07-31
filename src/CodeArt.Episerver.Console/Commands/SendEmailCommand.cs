using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using CodeArt.Episerver.DevConsole.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command("sendemail",Description ="Sends one or more emails", Group =CommandGroups.GENERAL)]
    public class SendEmailCommand : IConsoleCommand, IInputCommand
    {
        [CommandParameter]
        public string FromEmail { get; set; }
        [CommandParameter]
        public string FromName { get; set; }
        [CommandParameter]
        public string ToEmail { get; set; }
        [CommandParameter]
        public string ToName { get; set; }
        [CommandParameter]
        public string Subject { get; set; }
        [CommandParameter]
        public string Body { get; set; }
        [CommandParameter]
        public bool IsHtml { get; set; }

        private TransferFile _attachment;

        private SmtpClient _smtpClient;

        private int _count;

        public SendEmailCommand()
        {
            _smtpClient = new SmtpClient();
            _count = 0;
        }

        public string Execute(params string[] parameters)
        {
            if (_count == 0) SendEmail();
            return $"Sent {_count} emails";
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            Source.OnCommandOutput += Source_OnCommandOutput;            
        }

        private void SendEmail()
        {
            MailAddress frm = new MailAddress(FromEmail, FromName);
            MailAddress to = new MailAddress(ToEmail, ToName);
            MailMessage message = new MailMessage(frm, to);
            message.Subject = Subject;
            message.Body = Body;
            message.IsBodyHtml = IsHtml;
            if (_attachment != null) {
                MemoryStream ms = new MemoryStream(_attachment.Data);
                Attachment a = new Attachment(ms,_attachment.FileName);
                message.Attachments.Add(a);
            }
            _smtpClient.Send(message);
            _count++;
        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            if(output is IDictionary<string, object>)
            {
                var dic = output as IDictionary<string, object>;
                if (dic.ContainsKey("FromEmail")) FromEmail = (string)dic["FromEmail"];
                if (dic.ContainsKey("FromName")) FromName = (string)dic["FromName"];
                if (dic.ContainsKey("ToEmail")) ToEmail = (string)dic["ToEmail"];
                if (dic.ContainsKey("ToName")) ToEmail = (string)dic["ToName"];
                if (dic.ContainsKey("Attachment")) _attachment = (TransferFile)dic["Attachement"];
                if (dic.ContainsKey("Subject")) Subject = (string)dic["Subject"];
                if (dic.ContainsKey("Body")) Body = (string)dic["Body"];

            } else if(output is string)
            {
                if (string.IsNullOrEmpty(Subject)) Subject = output as string;
                else if (string.IsNullOrEmpty(Body)) Body = output as string;
            } else if(output is TransferFile)
            {
                _attachment = output as TransferFile;
            }
            SendEmail();
        }
    }
}

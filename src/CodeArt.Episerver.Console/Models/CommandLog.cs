using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeArt.Episerver.DevConsole.Models
{
    public class CommandLog
    {
        public DateTime Time{ get; set; }
        public string Sender { get; set; }
        public string Text { get; set; }

        public string TimeString { get { return Time.ToShortTimeString(); } }

        public CommandLog(string Sender, string Text)
        {
            Time = DateTime.Now;
            this.Sender = Sender;
            this.Text = Text;
        }
        public override string ToString()
        {
            return Time.ToShortTimeString() + " " + Sender + "> " + Text;
        }
    }
}
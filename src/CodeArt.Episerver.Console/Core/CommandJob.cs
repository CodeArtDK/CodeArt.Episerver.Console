using CodeArt.Episerver.DevConsole.Models;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace CodeArt.Episerver.DevConsole.Core
{
    public class CommandJob
    {
        public string OriginalCommand { get; set; }
        public Dictionary<string,string> Arguments { get; set; }
        public ConcurrentQueue<object> Incoming { get; set; }
        public ConcurrentQueue<CommandLog> LogQueue { get; set; }
        public ConcurrentQueue<object> Outgoing { get; set; }
        public string Owner { get; set; }
        public string Name { get; set; }
        public Thread Thread { get; set; }
        public Action<CommandJob> ToExecute { get; set; }

        public Injected<CommandManager> Manager { get; set; }

        public virtual void Execute()
        {
            ToExecute(this);
        }

        public CommandJob()
        {
            Arguments = new Dictionary<string, string>();
            Incoming = new ConcurrentQueue<object>();
            LogQueue = new ConcurrentQueue<CommandLog>();
            Outgoing = new ConcurrentQueue<object>();
        }

    }
}
using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Core;
using CodeArt.Episerver.DevConsole.Interfaces;
using CodeArt.Episerver.DevConsole.Models;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command(Keyword = "clear", Description ="Clears the log for this session")]
    public class ClearCommand : IConsoleCommand, ILogAwareCommand
    {
        public List<CommandLog> Log { get; set; }

        public string Execute(params string[] parameters)
        {
            Log.Clear();
            return "Log cleared";
        }
    }
}

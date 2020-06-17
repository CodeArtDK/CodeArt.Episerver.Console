using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Core;
using CodeArt.Episerver.DevConsole.Interfaces;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command(Keyword = "clear")]
    public class ClearCommand : IConsoleCommand
    {
        public string Execute(params string[] parameters)
        {
            var man = ServiceLocator.Current.GetInstance<CommandManager>();
            man.Log.Clear();
                
            return "Log cleared";
        }
    }
}

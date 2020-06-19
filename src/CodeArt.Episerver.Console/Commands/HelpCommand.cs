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
    [Command(Keyword = "help", Description ="Provides help using the console")]
    public class HelpCommand : IConsoleOutputCommand, IConsoleCommand
    {
        public event OutputToConsoleHandler OutputToConsole;

        public string Execute(params string[] parameters)
        {
            //TODO: Support a specific command in the parameters

            //Retrieve a list of commands
            var cmdMgr = ServiceLocator.Current.GetInstance<CommandManager>();
            foreach(var cmd in cmdMgr.Commands.Values)
            {
                OutputToConsole?.Invoke(this, cmd.Keyword);
                if(cmd.Info.Description!=null)  OutputToConsole?.Invoke(this, "\t"+cmd.Info.Description);
            }
            return null;
        }
    }
}

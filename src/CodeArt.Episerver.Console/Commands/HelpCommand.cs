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

        [CommandParameter]
        public string Command { get; set; }

        public string Execute(params string[] parameters)
        {
            //TODO: Support a specific command in the parameters
            if (string.IsNullOrEmpty(Command) && parameters.Length > 0) Command = parameters.First();
            var cmdMgr = ServiceLocator.Current.GetInstance<CommandManager>();

            if (!string.IsNullOrEmpty(Command))
            {
                var cmd = cmdMgr.Commands[Command];
                OutputToConsole?.Invoke(this, "Command: " + cmd.Keyword);
                OutputToConsole?.Invoke(this, "Description: " + cmd.Info.Description);
                OutputToConsole?.Invoke(this, "Parameters:");
                foreach( var p in cmd.Parameters.Keys)
                {
                    OutputToConsole?.Invoke(this, $"\t{cmd.Parameters[p].PropertyType.Name} {p}");

                }
            } else
            {
                //Retrieve a list of commands
                foreach (var cmd in cmdMgr.Commands.Values.OrderBy(cm => cm.Keyword))
                {
                    OutputToConsole?.Invoke(this, cmd.Keyword);
                    if (cmd.Info.Description != null) OutputToConsole?.Invoke(this, "\t" + cmd.Info.Description);
                }
            }
            
            return null;
        }
    }
}

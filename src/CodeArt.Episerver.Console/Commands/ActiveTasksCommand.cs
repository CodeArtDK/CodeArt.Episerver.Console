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
    [Command(Keyword = "activetasks", Description = "Lists all active tasks")]
    public class ActiveTasksCommand : IConsoleCommand, IConsoleOutputCommand
    {


        public event OutputToConsoleHandler OutputToConsole;

        public Injected<CommandManager> Manager { get; set; }

        public string Execute(params string[] parameters)
        {

            int cnt = 0;
            OutputToConsole?.Invoke(this, "Active Tasks: ");
            foreach(var t in Manager.Service.Tasks)
            {
                OutputToConsole?.Invoke(this, $"{t.Id}\t{t.Status}\t{t.Exception.Message}");
                cnt++;
            }
            return $"Listed {cnt} tasks";
        }
    }
}

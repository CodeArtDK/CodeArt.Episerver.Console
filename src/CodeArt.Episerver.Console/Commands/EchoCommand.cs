using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command(Keyword ="echo",Syntax = "echo [content to echo]", Description ="Echo's the provided string into piped commands")]
    public class EchoCommand : IConsoleCommand, IOutputCommand
    {
        public event CommandOutput OnCommandOutput;


        public string Execute(params string[] parameters)
        {
            foreach(var s in parameters)
                OnCommandOutput?.Invoke(this,s);
            return null;
        }
    }
}

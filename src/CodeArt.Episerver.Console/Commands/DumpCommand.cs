using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command(Keyword = "dump")]
    public class DumpCommand : IInputCommand, IConsoleOutputCommand
    {
        public IOutputCommand Source { get; set; }

        public event OutputToConsoleHandler OutputToConsole;

        public string Execute(params string[] parameters)
        {
            if (Source != null)
            {
                Source.OnCommandOutput += Source_OnCommandOutput;
            }

            return string.Empty;
        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            
            OutputToConsole?.Invoke(this, output?.ToString());
            
            //TODO: Support objects to show like tables?
        }
    }
}

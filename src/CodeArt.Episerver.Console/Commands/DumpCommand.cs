using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using Newtonsoft.Json;
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

        [CommandParameter]
        public DumpType Type { get; set; }

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

            if (Type == DumpType.Json)
            {
                OutputToConsole?.Invoke(this, JsonConvert.SerializeObject(output,new JsonSerializerSettings() { ReferenceLoopHandling=ReferenceLoopHandling.Ignore, Formatting=Formatting.Indented }));
            } else if(output is KeyValuePair<string, string>)
            {
                OutputToConsole?.Invoke(this, ((KeyValuePair<string, string>)output).Key+": "+ ((KeyValuePair<string, string>)output).Value);
            } else  OutputToConsole?.Invoke(this, output?.ToString());
            
            //TODO: Support objects to show like tables?
        }
    }

    public enum DumpType
    {
        Json,
        List,
        Xml,
        String
    }
}

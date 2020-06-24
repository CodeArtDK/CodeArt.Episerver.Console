using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command(Keyword ="tolist", Description = "Aggregates piped input to a list and passes it on")]
    public class ToListCommand : IConsoleCommand, IOutputCommand, IInputCommand
    {
        public event CommandOutput OnCommandOutput;

        public List<Object> List { get; set; }
        public string Execute(params string[] parameters)
        {
            OnCommandOutput?.Invoke(this, List);
            return null;
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            List = new List<object>();
            Source.OnCommandOutput += Source_OnCommandOutput;
        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            List.Add(output);
        }
    }
}

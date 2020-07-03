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
    [Command(Keyword ="tojson", Description ="Converts input to Json")]
    public class ToJsonCommand : IConsoleCommand, IInputCommand, IOutputCommand
    {
        public event CommandOutput OnCommandOutput;

        public string Execute(params string[] parameters)
        {
            return null;
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            Source.OnCommandOutput += Source_OnCommandOutput;
        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            string json = JsonConvert.SerializeObject(output,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented
                });
            OnCommandOutput?.Invoke(this, json);
        }
    }
}

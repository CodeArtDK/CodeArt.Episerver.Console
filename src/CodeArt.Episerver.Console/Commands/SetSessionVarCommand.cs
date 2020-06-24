using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CodeArt.Episerver.DevConsole.Commands
{

    [Command(Keyword ="setsessionvar", Description ="Sets a session variable")]
    public class SetSessionVarCommand : IConsoleCommand, IInputCommand
    {
        [CommandParameter]
        public string Key { get; set; }

        [CommandParameter]
        public string Value { get; set; }

        public string Execute(params string[] parameters)
        {
            if (!string.IsNullOrEmpty(Value)) HttpContext.Current.Session[Key] = Value;
            return "Session variable stored";
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            Source.OnCommandOutput += Source_OnCommandOutput;
        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            HttpContext.Current.Session[Key] = output;
        }
    }
}

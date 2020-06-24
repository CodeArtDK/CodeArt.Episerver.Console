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
    [Command(Keyword ="getsessionvar", Description ="Gets a session variable and either passes it on, or outputs it")]
    public class GetSessionVarCommand : IConsoleCommand, IOutputCommand
    {
        public event CommandOutput OnCommandOutput;

        [CommandParameter]
        public string Key { get; set; }

        public string Execute(params string[] parameters)
        {
            object val = HttpContext.Current.Session[Key];
            if (OnCommandOutput == null)
            {
                return Key + ": " + val;
            }
            else OnCommandOutput.Invoke(this, val);

            return null;
        }
    }
}

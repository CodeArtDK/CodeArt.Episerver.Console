using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command(Keyword = "memory")]
    public class Memory : IConsoleCommand
    {
        public string Help => "";

        public string Execute(params string[] parameters)
        {

            Process p = Process.GetCurrentProcess();
            long mem = p.PrivateMemorySize64;

            if (parameters.Length > 0)
            {
                if (parameters.First() == "mb") mem = (int)mem / (1024 * 1024);
            }


            return "Private Memory Used: " + mem + " "+parameters.FirstOrDefault();
        }
    }
}

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

            return "Private Memory Used: " + p.PrivateMemorySize64;
        }
    }
}

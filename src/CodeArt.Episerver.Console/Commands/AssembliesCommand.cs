using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command(Keyword ="assemblies", Description = "List all assemblies loaded in the AppDomain")]
    public class AssembliesCommand : IOutputCommand, IConsoleOutputCommand
    {
        public event CommandOutput OnCommandOutput;
        public event OutputToConsoleHandler OutputToConsole;

        public string Execute(params string[] parameters)
        {
            int cnt = 0;
            if (OnCommandOutput == null) OutputToConsole.Invoke(this, "Assemblies loaded: ");
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                cnt++;
                if (OnCommandOutput != null)
                {
                    //Pass on
                    OnCommandOutput.Invoke(this, assembly);

                } else
                {
                    //Output
                    OutputToConsole.Invoke(this, "\t" + assembly.FullName);
                }
            }
            return $"{cnt} assemblies listed.";
        }
    }
}

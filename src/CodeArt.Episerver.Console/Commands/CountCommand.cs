using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command("count",Description ="Counts how many times it has received input and outputs it to the log")]
    public class CountCommand : IConsoleCommand, IInputCommand, IOutputCommand
    {
        private int _count;

        public event CommandOutput OnCommandOutput;

        public string Execute(params string[] parameters)
        {
            return _count.ToString();
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            _count = 0;
            Source.OnCommandOutput += Source_OnCommandOutput;
        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            _count++;
            OnCommandOutput?.Invoke(sender, output);
        }
    }
}

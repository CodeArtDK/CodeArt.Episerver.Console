using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command(Keyword = "take")]
    public class TakeCommand : IOutputCommand, IInputCommand
    {
        public IOutputCommand Source { get; set; }

        public event CommandOutput OnCommandOutput;

        [CommandParameter]
        public int Count { get; set; }

        private int _taken;

        public string Execute(params string[] parameters)
        {
            _taken = 0;
            if (parameters.Length == 1) Count = int.Parse(parameters[0]);
            if (Source != null)
            {
                Source.OnCommandOutput += Source_OnCommandOutput;
            }

            return string.Empty;
        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            if (_taken < Count)
            {
                OnCommandOutput?.Invoke(this, output);
                _taken++;
            }
        }
    }
}

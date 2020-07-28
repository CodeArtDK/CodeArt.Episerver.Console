using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command("readcsvfile", Description ="Used to read a CSV file. Will produce a dictionary of each row and pipe on afterwards.")]
    public class ReadCsvFileCommand : IInputCommand, IOutputCommand, IConsoleCommand
    {
        public event CommandOutput OnCommandOutput;

        [CommandParameter]
        public string Separator { get; set; }

        public ReadCsvFileCommand()
        {
            Separator = ";";
        }

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

        }
    }
}

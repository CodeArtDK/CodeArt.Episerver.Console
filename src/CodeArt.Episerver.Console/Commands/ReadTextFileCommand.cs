using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using CodeArt.Episerver.DevConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command("readtextfile", Description ="Reads a text file - for example provided by upload and produces text")]
    public class ReadTextFileCommand : IConsoleCommand, IInputCommand, IOutputCommand
    {
        public event CommandOutput OnCommandOutput;

        [CommandParameter(Description ="Return a single string with all text")]
        public bool SingleString { get; set; }


        private TransferFile transferFile;
        public string Execute(params string[] parameters)
        {
            if (transferFile == null) return "No file provided";
            else
            {
                string txt = UTF8Encoding.UTF8.GetString(transferFile.Data);
                if (SingleString) OnCommandOutput?.Invoke(this, txt);
                else txt.Split('\n').ToList().ForEach(l =>  OnCommandOutput?.Invoke(this, l));
            }
            return $"Successfully read {transferFile.FileName}";
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            Source.OnCommandOutput += Source_OnCommandOutput;
        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            if (output is TransferFile) transferFile = output as TransferFile;
        }
    }
}

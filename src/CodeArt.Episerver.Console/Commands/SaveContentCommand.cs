using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command(Keyword ="savecontent")]
    public class SaveContentCommand : IConsoleCommand, IInputCommand, IOutputCommand
    {

        public event CommandOutput OnCommandOutput;

        private readonly IContentRepository _repo;


        [CommandParameter]
        public EPiServer.DataAccess.SaveAction Action { get; set; }

        public SaveContentCommand(IContentRepository contentRepository)
        {
            _repo = contentRepository;
            Action = EPiServer.DataAccess.SaveAction.Publish;

        }
        public string Execute(params string[] parameters)
        {
            //Prepare to save content
            return null;
        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            var content = output as IContent;
            var r = _repo.Save(content, Action);
            OnCommandOutput?.Invoke(this, r);
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            if (Source != null) Source.OnCommandOutput += Source_OnCommandOutput;
        }

    }
}

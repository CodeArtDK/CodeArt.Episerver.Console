using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using EPiServer;
using EPiServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command("movecontent", Description = "Permanently moves content")]
    public class MoveContentCommand : IConsoleCommand, IInputCommand
    {

        private readonly IContentRepository _repo;

        private int _count;

        [CommandParameter]
        public string Content { get; set; }

        [CommandParameter(Required =true)]
        public string Destination { get; set; }

        public MoveContentCommand(IContentRepository repo)
        {
            _repo = repo;
            _count = 0;
        }

        private void Move(ContentReference cref)
        {
            _repo.Move(cref, ContentReference.Parse(Destination));
            _count++;
        }

        public string Execute(params string[] parameters)
        {
            if (!string.IsNullOrEmpty(Content)) Move(ContentReference.Parse(Content));
            else if (parameters.Length > 0)
            {
                foreach (var p in parameters) Move(ContentReference.Parse(p));
            }
            return $"Completed. {_count} content items moved";
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            Source.OnCommandOutput += Source_OnCommandOutput;

        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            if (output is ContentReference) Move((ContentReference)output);
            else if (output is IContent) Move((output as IContent).ContentLink);
        }
    }
}

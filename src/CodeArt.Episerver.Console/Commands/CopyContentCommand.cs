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
    [Command("copycontent", Description = "Permanently copies content. If no destination is specified, it is copied to the same parent. Output is the content reference of the new item.")]
    public class CopyContentCommand : IConsoleCommand, IInputCommand, IOutputCommand
    {

        private readonly IContentRepository _repo;

        private int _count;

        public event CommandOutput OnCommandOutput;

        [CommandParameter]
        public string Content { get; set; }

        [CommandParameter]
        public string Destination { get; set; }

        public CopyContentCommand(IContentRepository repo)
        {
            _repo = repo;
            _count = 0;
        }

        private void Copy(ContentReference cref)
        {
            var dest = (!string.IsNullOrEmpty(Destination) ? ContentReference.Parse(Destination) :_repo.Get<IContent>(cref).ParentLink);
            var cf=_repo.Copy(cref, dest);
            OnCommandOutput?.Invoke(this,cf);
            _count++;
        }

        public string Execute(params string[] parameters)
        {
            if (!string.IsNullOrEmpty(Content)) Copy(ContentReference.Parse(Content));
            else if (parameters.Length > 0)
            {
                foreach (var p in parameters) Copy(ContentReference.Parse(p));
            }
            return $"Completed. {_count} content items copied";
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            Source.OnCommandOutput += Source_OnCommandOutput;

        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            if (output is ContentReference) Copy((ContentReference)output);
            else if (output is IContent) Copy((output as IContent).ContentLink);
        }
    }
}

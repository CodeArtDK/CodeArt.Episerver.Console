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
    [Command("deletecontent", Description = "Permanently deletes content")]
    public class DeleteContentCommand : IConsoleCommand, IInputCommand
    {

        private readonly IContentRepository _repo;

        private int _count;

        [CommandParameter]
        public string Content { get; set; }

        [CommandParameter]
        public bool Force { get; set; }

        public DeleteContentCommand(IContentRepository repo)
        {
            _repo = repo;
            _count = 0;
        }

        private void Delete(ContentReference cref)
        {
            _repo.Delete(cref,Force);
            _count++;
        }

        public string Execute(params string[] parameters)
        {
            if (!string.IsNullOrEmpty(Content)) Delete(ContentReference.Parse(Content));
            else if (parameters.Length > 0)
            {
                foreach (var p in parameters) Delete(ContentReference.Parse(p));
            }
            return $"Completed. {_count} content items deleted";
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            Source.OnCommandOutput += Source_OnCommandOutput;

        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            if (output is ContentReference) Delete((ContentReference)output);
            else if (output is IContent) Delete((output as IContent).ContentLink);
        }
    }
}

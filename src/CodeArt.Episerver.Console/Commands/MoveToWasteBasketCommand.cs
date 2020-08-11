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
    [Command("movetowastebasket", Description ="Moves content to the waste basket")]
    public class MoveToWasteBasketCommand : IConsoleCommand, IInputCommand
    {

        private readonly IContentRepository _repo;

        private int _count;

        [CommandParameter]
        public string Content { get; set; }

        public MoveToWasteBasketCommand(IContentRepository repo)
        {
            _repo = repo;
            _count = 0;
        }

        private void MoveToWasteBasket(ContentReference cref)
        {
            _repo.MoveToWastebasket(cref);
            _count++;
        }

        public string Execute(params string[] parameters)
        {
            if (!string.IsNullOrEmpty(Content)) MoveToWasteBasket(ContentReference.Parse(Content));
            else if (parameters.Length > 0)
            {
                foreach (var p in parameters) MoveToWasteBasket(ContentReference.Parse(p));
            }
            return $"Completed. {_count} content items moved to waste basket";
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            Source.OnCommandOutput += Source_OnCommandOutput;

        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            if (output is ContentReference) MoveToWasteBasket((ContentReference)output);
            else if (output is IContent) MoveToWasteBasket((output as IContent).ContentLink);
        }
    }
}

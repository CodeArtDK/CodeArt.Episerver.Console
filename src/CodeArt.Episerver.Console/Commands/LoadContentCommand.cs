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
    /// <summary>
    /// Loads IContent from a content reference.
    /// Used for piping.
    /// </summary>
    [Command(Keyword ="loadcontent",Description ="Loads content from a content reference", Group = CommandGroups.CONTENT)]
    public class LoadContentCommand : IInputCommand, IOutputCommand
    {
        public event CommandOutput OnCommandOutput;

        private readonly IContentRepository _repo;

        public LoadContentCommand(IContentRepository contentRepository)
        {
            _repo = contentRepository;
        }

        public string Execute(params string[] parameters)
        {
            if (parameters.Length > 0)
            {
                foreach(var p in parameters)
                {
                    ContentReference cr = ContentReference.Parse(p);
                    var c = _repo.Get<IContent>(cr);
                    OnCommandOutput?.Invoke(this, c);

                }
            }
            return null;
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            Source.OnCommandOutput += Source_OnCommandOutput;
        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            ContentReference cr = (output.GetType() == typeof(ContentReference)) ? (ContentReference)output : ContentReference.Parse(output.ToString());
            var c = _repo.Get<IContent>(cr);
            OnCommandOutput?.Invoke(this, c);
        }
    }
}

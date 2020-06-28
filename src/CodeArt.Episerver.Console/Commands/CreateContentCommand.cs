using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command(Keyword ="createcontent",Description ="Creates a new content element", Group =CommandGroups.CONTENT)]
    public class CreateContentCommand : IConsoleCommand, IOutputCommand
    {
        public event CommandOutput OnCommandOutput;

        [CommandParameter]
        public string ContentTypeName { get; set; }

        [CommandParameter]
        public int ContentTypeID { get; set; }

        [CommandParameter]
        public string Parent { get; set; }


        private readonly IContentRepository _repo;
        private readonly IContentTypeRepository _trepo;

        public CreateContentCommand(IContentTypeRepository contentTypeRepository, IContentRepository contentRepository)
        {
            _repo = contentRepository;
            _trepo = contentTypeRepository;
        }

        public string Execute(params string[] parameters)
        {
            ContentType ct = null;
            if (!string.IsNullOrEmpty(ContentTypeName)) ct = _trepo.Load(ContentTypeName);
            else if (ContentTypeID != 0) ct = _trepo.Load(ContentTypeID);
            else if (parameters.Length >0) ct = _trepo.Load(parameters.First());
            else return "No Content Type specified";

            ContentReference pref = ContentReference.EmptyReference;
            if (!string.IsNullOrEmpty(Parent)) pref = ContentReference.Parse(Parent);
            else if (parameters.Length == 2) pref = ContentReference.Parse(parameters.Last());
            else return "No Parent specified";

            var content=_repo.GetDefault<IContent>(pref, ct.ID);
            OnCommandOutput?.Invoke(this, content);
            return $"Content of type {ct.Name} created below {pref} and passed to the pipe.";

        }
    }
}

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


        protected Injected<IContentRepository> repo { get; set; }
        protected Injected<IContentTypeRepository> trepo { get; set; }


        public string Execute(params string[] parameters)
        {
            ContentType ct = null;
            if (!string.IsNullOrEmpty(ContentTypeName)) ct = trepo.Service.Load(ContentTypeName);
            else if (ContentTypeID != 0) ct = trepo.Service.Load(ContentTypeID);
            else if (parameters.Length >0) ct = trepo.Service.Load(parameters.First());
            else return "No Content Type specified";

            ContentReference pref = ContentReference.EmptyReference;
            if (!string.IsNullOrEmpty(Parent)) pref = ContentReference.Parse(Parent);
            else if (parameters.Length == 2) pref = ContentReference.Parse(parameters.Last());
            else return "No Parent specified";

            var content=repo.Service.GetDefault<IContent>(pref, ct.ID);
            OnCommandOutput?.Invoke(this, content);
            return $"Content of type {ct.Name} created below {pref} and passed to the pipe.";

        }
    }
}

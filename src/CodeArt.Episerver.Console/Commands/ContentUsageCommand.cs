using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command("contentusage", Description ="Finds references to a piece of content", Group =CommandGroups.CONTENT)]
    public class ContentUsageCommand : IConsoleCommand, IInputCommand, IOutputCommand
    {
        public event CommandOutput OnCommandOutput;

        private readonly IContentSoftLinkRepository _softLinksRepo;
        private readonly IContentRepository _repo;

        [CommandParameter(Description ="If true, will show links pointing to the content item - otherwise links going from the content item.")]
        public bool Incoming { get; set; }

        [CommandParameter(Description = "If set, will use this content item")]
        public string Content { get; set; }

        public ContentUsageCommand(IContentSoftLinkRepository softLinkRepository, IContentRepository repository)
        {
            _repo = repository;
            _softLinksRepo = softLinkRepository;

        }

        private void LookupSoftlinks(ContentReference cref)
        {
            var softlinks=_softLinksRepo.Load(cref, Incoming);
            OnCommandOutput.Invoke(this, softlinks);
        }

        public string Execute(params string[] parameters)
        {
            if (!string.IsNullOrEmpty(Content))
            {
                LookupSoftlinks(ContentReference.Parse(Content));
            }
            return null;
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            Source.OnCommandOutput += Source_OnCommandOutput;
        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            if (output is ContentReference) LookupSoftlinks((ContentReference)output);
            if (output is IContent) LookupSoftlinks(((IContent)output).ContentLink);
            //if(output is IDictionary<string,object>)    
        }
    }
}
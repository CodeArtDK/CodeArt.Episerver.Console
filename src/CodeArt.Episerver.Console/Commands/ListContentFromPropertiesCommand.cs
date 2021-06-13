using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AddToProjectSite.AddToProject
{
    [Command(Keyword = "listcontentfromproperties", Description = "Get dependent content types")]
    public class GetDependentsCommand : IConsoleCommand, IOutputCommand
    {
        [CommandParameter]
        public string Parent{ get; set; }

        public event CommandOutput OnCommandOutput;

        private readonly IContentSoftLinkRepository contentSoftLinkRepository;

        private readonly IContentRepository contentRepository;

        //Analyze recursive!!
        private IEnumerable<IContent> ListChildElements(IContent c)
        {
            foreach(var p in c.Property.Where(pp => !pp.Name.StartsWith("Page")))
            {
                if (p.Type == PropertyDataType.Block)
                {
                    //contentRepository.Get<IContent>()
                } else if (p.Type == PropertyDataType.ContentReference)
                {
                    yield return contentRepository.Get<IContent>((ContentReference)p.Value);
                }
                else if(p.Value is ContentArea) //ContentArea, XHTML, ??
                {
                    var lst=(p.Value as ContentArea).Items.Select(i => contentRepository.Get<IContent>(i.ContentLink));
                    //Should we recursively handle block in blocks?
                    foreach (var l in lst) yield return l;

                } else if(p.Value is XhtmlString)
                {
                    //TODO, handle
                }

            }
            //Nothing to return
        }

        public GetDependentsCommand(IContentSoftLinkRepository contentSoftLinkRepository, IContentRepository contentRepository)
        {
            this.contentSoftLinkRepository = contentSoftLinkRepository;
            this.contentRepository = contentRepository;
        }
        public string Execute(params string[] parameters)
        {
            int cnt = 0;

            if (string.IsNullOrEmpty(Parent)) Parent = parameters.First();

            ContentReference cr = ContentReference.Parse(Parent);

            var lst=ListChildElements(contentRepository.Get<IContent>(cr)).Distinct().ToList();

            cnt = lst.Count();

            foreach (var l in lst) OnCommandOutput.Invoke(this, l);
            //var lst =contentSoftLinkRepository.Load(cr).Where(cs => cs.OwnerContentLink.CompareToIgnoreWorkID(cr));
            //foreach (var l in lst.Select(ll => ll.ReferencedContentLink))
            //{
            //    cnt++;
            //    OnCommandOutput.Invoke(this, contentRepository.Get<IContent>(l));
            //}

            return $"Found {cnt} dependents";
        }
    }
}
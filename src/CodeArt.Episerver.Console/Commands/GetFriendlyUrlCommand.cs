using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command(Keyword ="getfriendlyurl", Description ="Fetches the relative, friendly url to a piece of content")]
    public class GetFriendlyUrlCommand : IConsoleCommand, IInputCommand, IOutputCommand
    {
        public event CommandOutput OnCommandOutput;

        public Injected<UrlResolver> UrlResolver { get; set; }

        [CommandParameter]
        public string Content { get; set; }

        [CommandParameter]
        public string Language { get; set; }

        private string FindUrl(ContentReference cref)
        {
            if (!string.IsNullOrEmpty(Language)) return UrlResolver.Service.GetUrl(cref, Language);
            else return UrlResolver.Service.GetUrl(cref);
        }
        public string Execute(params string[] parameters)
        {
            if (!string.IsNullOrEmpty(Content)) return FindUrl(ContentReference.Parse(Content));
            if (parameters.Length == 1) return FindUrl(ContentReference.Parse(parameters[0]));
            return null;
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            Source.OnCommandOutput += Source_OnCommandOutput;
        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            if (output is IContent) OnCommandOutput?.Invoke(this, FindUrl(((IContent)output).ContentLink));
            if (output is ContentReference) OnCommandOutput?.Invoke(this, FindUrl((ContentReference)output));
            if (output is string) OnCommandOutput?.Invoke(this, FindUrl(ContentReference.Parse((string)output)));
            if (output is int) OnCommandOutput?.Invoke(this, FindUrl(new ContentReference((int)output)));
        }

        public GetFriendlyUrlCommand()
        {
        }
    }
}

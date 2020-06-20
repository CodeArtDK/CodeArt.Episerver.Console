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

namespace DeveloperTools.Console. Commands
{
    [Command(Keyword = "listdescendents", Description ="Lists all descendents below the node")]
    public class ListDescendentsCommand : IOutputCommand
    {
        public event CommandOutput OnCommandOutput;

        [CommandParameter]
        public string Parent { get; set; }

        public string Execute(params string[] parameters)
        {
            var repo = ServiceLocator.Current.GetInstance<IContentRepository>();
            int cnt = 0;
            ContentReference start = ContentReference.StartPage;
            if (!string.IsNullOrEmpty(Parent))
            {
                if (Parent.ToLower() == "root") start = ContentReference.RootPage;
                start = ContentReference.Parse(Parent);
            }

            foreach(var r in repo.GetDescendents(start))
            {
                OnCommandOutput?.Invoke(this, repo.Get<IContent>(r));
                cnt++;
            }

            return $"Done, listing {cnt} content items";
        }
    }
}

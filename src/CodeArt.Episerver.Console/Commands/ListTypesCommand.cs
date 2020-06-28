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

namespace DeveloperTools.Console. Commands
{
    [Command(Keyword = "contenttypes", Description ="Lists all content types")]
    public class ListTypesCommand : IOutputCommand
    {
        public event CommandOutput OnCommandOutput;

        private readonly IContentTypeRepository _trepo;

        public ListTypesCommand(IContentTypeRepository contentTypeRepository)
        {
            _trepo = contentTypeRepository;
        }

        public string Execute(params string[] parameters)
        {
            int cnt = 0;
            
            foreach(var r in _trepo.List())
            {
                OnCommandOutput?.Invoke(this, r);
                cnt++;
            }

            return $"Done, listing {cnt} content types";
        }
    }
}

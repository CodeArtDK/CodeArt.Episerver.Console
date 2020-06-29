using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using EPiServer.Framework.Cache;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{

    [Command(Keyword ="objectcache", Description ="")]
    public class ObjectCacheCommand : IConsoleCommand, IOutputCommand, IConsoleOutputCommand
    {
        public event CommandOutput OnCommandOutput;
        public event OutputToConsoleHandler OutputToConsole;

        private readonly ISynchronizedObjectInstanceCache _cache;

        public ObjectCacheCommand(ISynchronizedObjectInstanceCache synchronizedObjectInstanceCache)
        {
            _cache = synchronizedObjectInstanceCache;
        }

        public string Execute(params string[] parameters)
        {
            
            throw new NotImplementedException();
        }
    }
}

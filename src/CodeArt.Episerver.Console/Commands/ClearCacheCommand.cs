using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using EPiServer.Framework.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command("clearcache", Description ="Clears the object cache - either completely, or for a given cache key")]
    public class ClearCacheCommand : IConsoleCommand, IInputCommand
    {
        private readonly ISynchronizedObjectInstanceCache _cache;

        public string CacheKey { get; set; }

        public ClearCacheCommand(ISynchronizedObjectInstanceCache cache)
        {
            _cache = cache;
            
        }
        public string Execute(params string[] parameters)
        {
            throw new NotImplementedException();
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            Source.OnCommandOutput += Source_OnCommandOutput;
        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            
        }
    }
}

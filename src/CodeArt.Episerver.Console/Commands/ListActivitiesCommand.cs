using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using EPiServer.DataAbstraction.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command("listactivities", Description ="List activities from the activity log")]
    public class ListActivitiesCommand : IConsoleCommand, IOutputCommand
    {
        public event CommandOutput OnCommandOutput;

        private readonly IActivityQueryService _queryService;

        public ListActivitiesCommand(IActivityQueryService activityQueryService)
        {
            _queryService = activityQueryService;
        }

        public string Execute(params string[] parameters)
        {
            //_queryService.ListActivitiesAsync(new ActivityQuery() {  })
            return null;
        }
    }
}

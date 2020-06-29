using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command(Keyword = "listen", Description = "Listens to a specific event")]
    public class ListenCommand : IOutputCommand
    {
        public event CommandOutput OnCommandOutput;

        [CommandParameter]
        public string Event { get; set; }

        private readonly IContentEvents _events;

        public ListenCommand(IContentEvents contentEvents)
        {
            _events = contentEvents;
        }


        public string Execute(params string[] parameters)
        {
            if (!string.IsNullOrEmpty(Event))
            {
                var ev=_events.GetType().GetEvent(Event);
                EventHandler<EPiServer.ContentEventArgs> eventHandler = new EventHandler<EPiServer.ContentEventArgs>(Events_ContentEvent);
                ev.AddEventHandler(_events, eventHandler);
                return $"Now listening for events on {Event}";
            }

            return $"You have to specify which event to listen to, using '-Event [eventname]'. Options are: "+string.Join(", ",_events.GetType().GetEvents().Select(e => e.Name).ToArray());
        }



        private void Events_ContentEvent(object sender, EPiServer.ContentEventArgs e)
        {
            OnCommandOutput?.Invoke(this, e);
        }
    }
}

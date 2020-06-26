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

        public Injected<IContentEvents> Events { get; set; }

        public string Execute(params string[] parameters)
        {
            if (!string.IsNullOrEmpty(Event))
            {
                var ev=Events.Service.GetType().GetEvent(Event);
                EventHandler<EPiServer.ContentEventArgs> eventHandler = new EventHandler<EPiServer.ContentEventArgs>(Events_ContentEvent);
                ev.AddEventHandler(Events.Service, eventHandler);
                return $"Now listening for events on {Event}";
            }

            return $"You have to specify which event to listen to, using '-Event [eventname]'. Options are: "+string.Join(", ",Events.Service.GetType().GetEvents().Select(e => e.Name).ToArray());
        }



        private void Events_ContentEvent(object sender, EPiServer.ContentEventArgs e)
        {
            OnCommandOutput?.Invoke(this, e);
        }
    }
}

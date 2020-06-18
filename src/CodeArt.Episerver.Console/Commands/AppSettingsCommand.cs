using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using EPiServer;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command(Keyword = "appsettings", Description = "Lists all app settings")]
    public class AppSettingsCommand : IOutputCommand
    {
        public event CommandOutput OnCommandOutput;

        [CommandParameter]
        public string Key { get; set; }


        public string Execute(params string[] parameters)
        {
            if (string.IsNullOrEmpty(Key) && parameters.Length > 0) Key = parameters.First();
            if (!string.IsNullOrEmpty(Key))
            {
                OnCommandOutput?.Invoke(this, ConfigurationManager.AppSettings[Key]);
                return $"Done. Appsetting \"{Key}\"=\"{ConfigurationManager.AppSettings[Key]}\".";
            }

            int cnt = 0;
            
            foreach (var r in ConfigurationManager.AppSettings)
            {
                OnCommandOutput?.Invoke(this, r);
                cnt++;
            }

            return $"Done, listing {cnt} appsettings";
        }
    }
}
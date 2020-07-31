using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command("listnuget", Description ="Lists nuget packages used")]
    public class ListNugetCommand : IConsoleCommand, IOutputCommand
    {
        public event CommandOutput OnCommandOutput;

        [CommandParameter]
        public bool OnlyName { get; set; }


        public string Execute(params string[] parameters)
        {
            string path = HttpContext.Current.Server.MapPath("~/packages.config");
            if (File.Exists(path))
            {
                int cnt = 0;
                var xdoc=XDocument.Parse(File.ReadAllText(path));
                foreach(var xe in xdoc.Descendants("package"))
                {
                    string name = xe.Attribute("id").Value;
                    string version = xe.Attribute("version").Value;
                    if (OnlyName) OnCommandOutput?.Invoke(this, name);
                    else
                    {
                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        dic.Add("Name", name);
                        dic.Add("Version", version);
                        OnCommandOutput?.Invoke(this, dic);
                    }
                    cnt++;
                }
                return $"Listed {cnt} packages";
            }
            return null;
        }
    }
}

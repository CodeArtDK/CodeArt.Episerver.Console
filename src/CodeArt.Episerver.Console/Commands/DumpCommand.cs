using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using EPiServer.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command(Keyword = "dump", Description ="Dumps objects to console log")]
    public class DumpCommand : IInputCommand, IConsoleOutputCommand
    {

        public event OutputToConsoleHandler OutputToConsole;

        [CommandParameter]
        public DumpType Type { get; set; }

        public string Execute(params string[] parameters)
        {
            return string.Empty;
        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {

            if (Type == DumpType.Json)
            {
                OutputToConsole?.Invoke(this, JsonConvert.SerializeObject(output, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, NullValueHandling=NullValueHandling.Ignore, Formatting = Formatting.Indented }));
            }
            else if (output is KeyValuePair<string, string>)
            {
                OutputToConsole?.Invoke(this, ((KeyValuePair<string, string>)output).Key + ": " + ((KeyValuePair<string, string>)output).Value);
            }
            else if (output is IContent)
            {
                var c = (IContent)output;
                OutputToConsole?.Invoke(this, "Content Object: " + c.Name);
                OutputToConsole?.Invoke(this, "\tKind: " + ((c is PageData)?"Page":(c is BlockData)? "Block": (c is MediaData)? "Media": "Other"));
                OutputToConsole?.Invoke(this, "\tParent: " + c.ParentLink.ToString());
                foreach(PropertyData p in (c as IContentData).Property){
                    OutputToConsole?.Invoke(this, "\t" + p.Name + ": " + HttpUtility.HtmlEncode(p.Value));
                }

            }
            else OutputToConsole?.Invoke(this, HttpUtility.HtmlEncode(output?.ToString()));
            
            //TODO: Support objects to show like tables?
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            if (Source != null)
            {
                Source.OnCommandOutput += Source_OnCommandOutput;
            }
        }
    }

    public enum DumpType
    {
        Default,
        Json,
        List,
        Xml,
        String
    }
}

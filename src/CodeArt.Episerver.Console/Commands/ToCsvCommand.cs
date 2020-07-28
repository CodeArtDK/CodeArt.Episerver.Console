using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using CodeArt.Episerver.DevConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command("tocsv", Description ="Produces a CSV file from the input")]
    public class ToCsvCommand : IConsoleCommand, IOutputCommand, IInputCommand
    {
        public event CommandOutput OnCommandOutput;

        private List<Dictionary<string, object>> content;

        [CommandParameter]
        public string Separator { get; set; }

        [CommandParameter]
        public bool ShowHeader { get; set; }


        public ToCsvCommand()
        {
            Separator = ";";
            ShowHeader = true;
        }

        public string Execute(params string[] parameters)
        {
            //Produce CSV file contents
            StringBuilder sb = new StringBuilder();
            var keys=content.SelectMany(kvp => kvp.Keys).Distinct().ToList();
            if (ShowHeader)
            {
                sb.AppendLine(string.Join(Separator, keys.Select(s => "\"" + s + "\"").ToArray()));
            }
            foreach(var l in content)
            {
                bool isFirst = true;
                foreach(var k in keys)
                {
                    if (!isFirst) sb.Append(Separator);
                    else isFirst = false;

                    if (l.ContainsKey(k))
                    {
                        object o = l[k];
                        if (o is int ||
                            o is bool ||
                            o is double) sb.Append(o.ToString());
                        else sb.Append("\"" + o.ToString() + "\"");
                    }
                }
                
                sb.AppendLine();
            }
            OnCommandOutput?.Invoke(this, sb.ToString());
            return null;
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            Source.OnCommandOutput += Source_OnCommandOutput;
            content = new List<Dictionary<string, object>>();
        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            //Accept list of dictionaries, dictionaries or objects
            if(output is List<Dictionary<string, object>>)
            {
                content = content.Union(output as List<Dictionary<string, object>>).ToList();
            } else if(output is Dictionary<string, object>)
            {
                content.Add(output as Dictionary<string, object>);
            }
            else
            {
                //outlook is object
                Dictionary<string, object> dic = new Dictionary<string, object>();
                foreach(var p in output.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)){
                    dic.Add(p.Name, p.GetValue(output));
                }
                content.Add(dic);
            }
        }
    }
}

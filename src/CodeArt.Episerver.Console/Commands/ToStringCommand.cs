using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command(Keyword ="tostring", Description ="Converts an object or a dictionary to string using a specified format. Use {} in the format to specify keys or property names.", Group =CommandGroups.GENERAL)]
    public class ToStringCommand : IConsoleCommand, IInputCommand, IOutputCommand
    {
        public event CommandOutput OnCommandOutput;

        [CommandParameter]
        public string Format { get; set; }

        public string Execute(params string[] parameters)
        {
            return null;
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            if (string.IsNullOrEmpty(Format) && parameters.Length > 0) Format = parameters.First();
            Source.OnCommandOutput += Source_OnCommandOutput;
        }
        private string GetElementFromSource(object output, string key)
        {
            string f = null;
            int idx = key.IndexOf(':');
            if ( idx> -1)
            {
                //We have a splitter
                f = key.Substring(idx+1);
                key = key.Substring(0, idx);
            }
            if(output is IDictionary<string, object>)
            {
                var obj = ((IDictionary<string, object>)output)[key];
                if (f != null)
                {
                    return (string)obj.GetType().GetMethods().Where(m => m.Name=="ToString")
                        .Where(m => m.GetParameters().Length==1)
                        .Where(m => m.GetParameters().First().ParameterType==typeof(string))
                        .FirstOrDefault().Invoke(obj, new object[] { f });
                }
                return obj.ToString();
                
            } else
            {
                var t = output.GetType();
                var obj = t.GetProperty(key).GetValue(output);
                if (f != null)
                {
                    return (string)obj.GetType().GetMethod("ToString").Invoke(obj, new object[] { f });
                }
                return obj.ToString();
            }
        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            if (Format == null) OnCommandOutput?.Invoke(this, output.ToString());
            else
            {
                string rt = Format;
                foreach(Match m in Regex.Matches(Format, @"{([a-zA-Z0-9]+\:?[^\}]*)}").Cast<Match>())
                {
                    string key = m.Groups[1].Value;
                    string val = GetElementFromSource(output, key);
                    //Replace in rt
                    rt=rt.Replace(m.Value, val);
                }
                OnCommandOutput?.Invoke(this, rt);
            }
        }
    }
}

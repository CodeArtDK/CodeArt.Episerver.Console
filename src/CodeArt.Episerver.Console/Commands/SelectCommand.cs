using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using EPiServer.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command(Keyword = "select", 
        Description = "Selects a specific property from a piece of content or an object or a dictionary.",
        Syntax ="[one or more property names]")]
    public class SelectCommand : IOutputCommand, IInputCommand
    {

        public event CommandOutput OnCommandOutput;

        [CommandParameter(Description ="The property to return. If multiple, use comma separatation")]
        public string Property { get; set; }

        protected List<string> properties { get; set; }

        public string Execute(params string[] parameters)
        {
            return string.Empty;
        }

        private string GetFirstProperty(string property)
        {
            return property.Split('.').First();
        }

        private string GetRemainingProperty(string property)
        {
            return string.Join(".", property.Split('.').Skip(1).ToArray());
        }

        private object FetchSingleProperty(IContent output, string property)
        {
            if (property.Contains("."))
            {
                return FetchSingleProperty(output.Property[GetFirstProperty(property)].Value, GetRemainingProperty(property));
            }
            return output.Property[property].Value;
        }

        private object FetchSingleProperty(IList output, string property)
        {
            if (property == "Count") return output.Count;
            var lst = new List<object>();
            foreach(var itm in output)
            {
                lst.Add(FetchSingleProperty(itm, property));
            }
            return lst;
        }

        private object FetchSingleProperty(IDictionary<string,object> output, string property)
        {
            if (property.Contains("."))
            {
                return FetchSingleProperty(output[GetFirstProperty(property)], GetRemainingProperty(property));
            }

            return output[property];
        }

        private object FetchSingleProperty(object output, string property)
        {
            if (output is IContent) return FetchSingleProperty((output as IContent), property);
            if (output is IDictionary<string, object>) return FetchSingleProperty((output as IDictionary<string, object>), property);
            if (output is IList) return FetchSingleProperty((output as IList), property);
            var t = output.GetType();
            if (property.Contains("."))
            {
                
                return FetchSingleProperty(t.GetProperty(GetFirstProperty(property)).GetValue(output), GetRemainingProperty(property));
            }
            return t.GetProperty(GetFirstProperty(property)).GetValue(output);
        }

        private void GetPropertiesAndPassOn(object output)
        {
            if (properties.Count == 1)
                OnCommandOutput?.Invoke(this, FetchSingleProperty(output, properties.First()));
            else
            {
                Dictionary<string, object> rt = new Dictionary<string, object>();
                foreach (var p in properties) rt.Add(p, FetchSingleProperty(output, p));
                OnCommandOutput?.Invoke(this, rt);
            }
        }

 

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            GetPropertiesAndPassOn(output);
            
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            if (parameters.Length > 0 && string.IsNullOrEmpty(Property))
            {
                properties = parameters.ToList();
            } else if (!string.IsNullOrEmpty(Property))
            {
                properties = Property.Split(',').ToList();
            }
            Source.OnCommandOutput += Source_OnCommandOutput;
        }
    }
}

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
    [Command(Keyword = "select", Description = "Selects a specific property from a piece of content or an object or a dictionary.")]
    public class SelectCommand : IOutputCommand, IInputCommand
    {

        public event CommandOutput OnCommandOutput;

        [CommandParameter]
        public string Property { get; set; } //TODO: Support multiple properties


        public string Execute(params string[] parameters)
        {
            

            return string.Empty;
        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            if (output is IContent)
            {
                OnCommandOutput?.Invoke(this, ((IContent)output).Property[Property].Value);
            }
            else if (output is IDictionary)
            {
                if (((IDictionary)output).Contains(Property))
                {
                    OnCommandOutput?.Invoke(this, ((IDictionary)output)[Property]);
                }
            }
            else if (output.GetType().IsClass)
            {
                var t = output.GetType();
                if (t != null)
                {
                    var p = t.GetProperty(Property);
                    if (p != null)
                    {
                        OnCommandOutput?.Invoke(this, p.GetValue(output));
                    }
                }
            }
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            if (parameters.Length > 0 && string.IsNullOrEmpty(Property))
            {
                Property = parameters.First();
            }
            Source.OnCommandOutput += Source_OnCommandOutput;
        }
    }
}

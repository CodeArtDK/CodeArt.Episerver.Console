using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using EPiServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command(Keyword ="setproperty", Description ="Sets a property on a piece of content passed in")]
    public class SetPropertyCommand : IConsoleCommand, IInputCommand, IOutputCommand
    {
        public event CommandOutput OnCommandOutput;

        [CommandParameter(Required =true)]
        public string Name { get; set; }

        [CommandParameter(Required =true)]
        public string Value { get; set; }

        public string Execute(params string[] parameters)
        {
            return null;
        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            if(output is ContentData)
            {
                var content = output as ContentData;
                if (content.Property[Name].PropertyValueType == typeof(int))
                {
                    content.SetValue(Name, int.Parse(Value));
                } else if (content.Property[Name].PropertyValueType == typeof(bool))
                {
                    content.SetValue(Name, bool.Parse(Value));
                } else if (content.Property[Name].PropertyValueType == typeof(ContentReference))
                {
                    content.SetValue(Name, ContentReference.Parse(Value));
                } else if(content.Property[Name].PropertyValueType == typeof(XhtmlString))
                {
                    content.SetValue(Name, new XhtmlString(Value));
                }
                else 
                {
                    content.SetValue(Name, Value);
                }
            }
            OnCommandOutput?.Invoke(this, output);
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            if (Source != null) Source.OnCommandOutput += Source_OnCommandOutput;
        }
    }
}

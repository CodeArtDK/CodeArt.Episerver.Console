using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command(Keyword ="addtodictionary", Group =CommandGroups.CONTROL, Description ="Adds the key and value to a dictionary. If input is a dictionary it is extended.")]
    public class AddToDictionaryCommand : IOutputCommand, IConsoleCommand, IInputCommand
    {
        public event CommandOutput OnCommandOutput;

        [CommandParameter]
        public string Key { get; set; }

        [CommandParameter]
        public string Value { get; set; }


        private object _value;

        private IDictionary<string, object> _dictionary=new Dictionary<string,object>();

        public string Execute(params string[] parameters)
        {
            if (string.IsNullOrEmpty(Key) && parameters.Length > 0) Key = parameters[0];
            if (_value == null && !string.IsNullOrEmpty(Value)) _value = Value;
            if (_value == null && parameters.Length == 2) _value = parameters.Last();
            _dictionary[Key] = _value;
            OnCommandOutput?.Invoke(this, _dictionary);
            return null;
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            Source.OnCommandOutput += Source_OnCommandOutput;
        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            if (output is IDictionary<string, object>) _dictionary = output as IDictionary<string, object>;
            else _value = output;
        }
    }
}

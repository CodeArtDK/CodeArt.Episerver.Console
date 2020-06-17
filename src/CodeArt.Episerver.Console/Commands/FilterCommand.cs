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
    [Command(Keyword = "filter", Description = "Filters objects parsed in and only returns those matching the requirements.")]
    public class FilterCommand : IOutputCommand, IInputCommand
    {
        public IOutputCommand Source { get; set; }

        public event CommandOutput OnCommandOutput;

        [CommandParameter]
        public string Property { get; set; }
        [CommandParameter]
        public Operators Operator { get; set; }
        [CommandParameter]
        public string Value { get; set; }

        public string Execute(params string[] parameters)
        {
            //propertyname EQUAL CONTAINS MATCH NOTEQUAL GT LT value
            if(Source!=null) Source.OnCommandOutput += Source_OnCommandOutput;

            if (parameters.Length == 3)
            {
                Property = parameters[0];
                Operator = (Operators)Enum.Parse(typeof(Operators), parameters[1]);
                Value = parameters[2];
            }
            //TODO: Validation
            return null;
        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            //Evalutate
            if(output is IContent)
            {
                bool result = true;
                var val = ((IContent)output).Property[Property].Value;
                if (val == null && Operator != Operators.IsNull) result = false;
                else if (Operator == Operators.Contains && !(val as string).Contains(Value)) result = false;
                else if (Operator == Operators.Equals && (val as string) != Value) result = false;
                else if (Operator == Operators.NotEquals && (val as string) == Value) result = false;
                else if (Operator == Operators.Matches && !System.Text.RegularExpressions.Regex.IsMatch(val as string, Value)) result = false;
                if (result)
                {
                    OnCommandOutput?.Invoke(this, output);
                }
            } 
            //TODO: Support Dictionaries and objects and strings?
        }
    }

    public enum Operators
    {
        Equals,
        Contains,
        Matches,
        NotEquals,
        GreaterThan,
        LessThan,
        IsNull,
        IsNotNull
    }
}

using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using EPiServer.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command(Keyword = "filter", Description = "Filters objects parsed in and only returns those matching the requirements.")]
    public class FilterCommand : IOutputCommand, IInputCommand
    {

        public event CommandOutput OnCommandOutput;

        [CommandParameter]
        public string Property { get; set; }
        [CommandParameter]
        public Operators Operator { get; set; }
        [CommandParameter]
        public string Value { get; set; }

        public string Execute(params string[] parameters)
        {
            return null;
        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            object val = output;
            //Evalutate
            if (output is IContent)
            {
                
                val = ((IContent)output).Property[Property].Value;
            } else if(output is IDictionary<string,object>)
            {
                val = ((IDictionary<string, object>)output)[Property];
            } else if(output.GetType().GetProperties(System.Reflection.BindingFlags.Public| System.Reflection.BindingFlags.Instance| System.Reflection.BindingFlags.GetProperty).Any(p => p.Name.ToLower() == Property.ToLower()))
            {
                var pi = output.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetProperty).First(p => p.Name.ToLower() == Property.ToLower());
                val=pi.GetValue(output);
            }
            if (val is string)
            {
                bool result = true;
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
            else if (val is int)
            {
                if ((Operator == Operators.Equals && ((int)val) == int.Parse(Value)) ||
                    (Operator == Operators.GreaterThan && ((int)val) > int.Parse(Value)) ||
                    (Operator == Operators.LessThan && ((int)val) < int.Parse(Value)) ||
                    (Operator == Operators.NotEquals && ((int)val) != int.Parse(Value)))
                {
                    OnCommandOutput?.Invoke(this, output);
                }

            }
            else if (val is DateTime)
            {
                DateTime dt = DateTime.Parse(Value,CultureInfo.InvariantCulture);
                if ((Operator == Operators.Equals && ((DateTime)val) == dt ||
                    (Operator == Operators.GreaterThan && ((DateTime)val) > dt)) ||
                    (Operator == Operators.LessThan && ((DateTime)val) < dt) ||
                    (Operator == Operators.NotEquals && ((DateTime)val) != dt))
                {
                    OnCommandOutput?.Invoke(this, output);
                }
            }
            //TODO: Support bool, decimal, double, complex objects?
            //TODO: Support Dictionaries and objects and strings?
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            //propertyname EQUAL CONTAINS MATCH NOTEQUAL GT LT value
            if (Source != null) Source.OnCommandOutput += Source_OnCommandOutput;

            if (parameters.Length == 3)
            {
                Property = parameters[0];
                switch (parameters[1])
                {
                    case "=": Operator = Operators.Equals;break;
                    case "!=":Operator = Operators.NotEquals;break;
                    case ">":Operator = Operators.GreaterThan;break;
                    case "<":Operator = Operators.LessThan;break;
                    default: Operator = (Operators)Enum.Parse(typeof(Operators), parameters[1], true);break;
                }
                Value = parameters[2];
                if (Value == "null" && Operator == Operators.Equals) Operator = Operators.IsNull;
                if (Value == "null" && Operator == Operators.NotEquals) Operator = Operators.IsNotNull;
            }
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

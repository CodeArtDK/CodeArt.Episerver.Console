using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using EPiServer.Core;
using System;
using System.Collections;
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

        private object FetchSingleProperty(IEnumerable output, string property)
        {
            if (property == "Count") return output.Cast<object>().Count();
            var lst = new List<object>();
            foreach (var itm in output)
            {
                lst.Add(FetchSingleProperty(itm, property));
            }
            return lst;
        }

        private object FetchSingleProperty(IDictionary<string, object> output, string property)
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
            if (output is IEnumerable && !(output is string)) return FetchSingleProperty((output as IEnumerable), property);
            var t = output.GetType();
            if (property.Contains("."))
            {

                return FetchSingleProperty(t.GetProperty(GetFirstProperty(property)).GetValue(output), GetRemainingProperty(property));
            }
            return t.GetProperty(GetFirstProperty(property)).GetValue(output);
        }

        private bool Evaluate(object val)
        {
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
                    return true;
                }
            }
            else if (val is int)
            {
                if ((Operator == Operators.Equals && ((int)val) == int.Parse(Value)) ||
                    (Operator == Operators.GreaterThan && ((int)val) > int.Parse(Value)) ||
                    (Operator == Operators.LessThan && ((int)val) < int.Parse(Value)) ||
                    (Operator == Operators.NotEquals && ((int)val) != int.Parse(Value)))
                {
                    return true;
                }

            }
            else if (val is DateTime)
            {
                DateTime dt = DateTime.Parse(Value, CultureInfo.InvariantCulture);
                if ((Operator == Operators.Equals && ((DateTime)val) == dt ||
                    (Operator == Operators.GreaterThan && ((DateTime)val) > dt)) ||
                    (Operator == Operators.LessThan && ((DateTime)val) < dt) ||
                    (Operator == Operators.NotEquals && ((DateTime)val) != dt))
                {
                    return true;
                }
            }
            else if (val is Boolean)
            {
                bool v = bool.Parse(Value);
                if ((Operator == Operators.Equals && ((bool)val) == v ||
                    (Operator == Operators.NotEquals && ((bool)val) != v)))
                {
                    return true;
                }
            }
            else if (val is Double)
            {
                Double d = Double.Parse(Value, CultureInfo.InvariantCulture);
                if ((Operator == Operators.Equals && ((Double)val) == d ||
                    (Operator == Operators.GreaterThan && ((Double)val) > d)) ||
                    (Operator == Operators.LessThan && ((Double)val) < d) ||
                    (Operator == Operators.NotEquals && ((Double)val) != d))
                {
                    return true;
                }
            }
            return false;
        }
        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            object val = output;
            if (!string.IsNullOrEmpty(Property))
            {
                val = FetchSingleProperty(output, Property);
            }
            //Evalutate
            if (((val is IList<object>) && (val as IList<object>).Any(o => Evaluate(o))) || Evaluate(val))
            {
                OnCommandOutput?.Invoke(this, output);
            }
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

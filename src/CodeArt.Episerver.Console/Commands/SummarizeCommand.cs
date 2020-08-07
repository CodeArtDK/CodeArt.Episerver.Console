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
    [Command("summarize",Description ="Summarizes occurences of a property. Outputs a dictionary<string,int> with a count of the values")]
    public class SummarizeCommand : IConsoleCommand, IOutputCommand, IInputCommand
    {
        public event CommandOutput OnCommandOutput;

        [CommandParameter]
        public string Key { get; set; }

        [CommandParameter]
        public SummarizationType Type { get; set; }

        [CommandParameter]
        public SummarizationInterval Interval { get; set; }


        private Dictionary<string,int> _dictionary { get; set; }

        public string Execute(params string[] parameters)
        {
            OnCommandOutput?.Invoke(this, _dictionary.ToDictionary(d => d.Key, e => (object)e.Value));
            return null;
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            Source.OnCommandOutput += Source_OnCommandOutput;
            //if(string.IsNullOrEmpty(Key) && parameters.Length > 0)
            //{
            //    Key = parameters.First();
            //}
        }

        private void IntStatistics(int n)
        {
            if (!_dictionary.ContainsKey("SUM")) _dictionary.Add("SUM", n);
            else _dictionary["SUM"] += n;
            if (!_dictionary.ContainsKey("MIN")) _dictionary.Add("MIN", n);
            else if (_dictionary["MIN"] > n) _dictionary["MIN"] = n;
            if (!_dictionary.ContainsKey("MAX")) _dictionary.Add("MAX", n);
            else if (_dictionary["MAX"] < n) _dictionary["MAX"] = n;
            if (!_dictionary.ContainsKey("COUNT")) _dictionary.Add("COUNT", 1);
            else _dictionary["COUNT"]++;
            if (!_dictionary.ContainsKey("AVG")) _dictionary.Add("AVG", n);
            else _dictionary["AVG"] = (int)_dictionary["SUM"] / _dictionary["COUNT"];
        }

        

        private void DistinctCount(object o)
        {
            string str = o.ToString();
            if(o is DateTime)
            {
                if (Interval == SummarizationInterval.Day)
                {
                    str = ((DateTime)o).ToString("yyyy-MM-dd");
                } else if(Interval == SummarizationInterval.Month)
                {
                    str = ((DateTime)o).ToString("yyyy-MM");
                } else if (Interval == SummarizationInterval.Year)
                {
                    str = ((DateTime)o).ToString("yyyy");
                }
            }

            if (_dictionary.ContainsKey(str)) _dictionary[str]++;
            else _dictionary.Add(str, 1);
        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            //Handle
            if (output is IContentData)
            {
                object val = (output as IContentData).Property[Key]?.Value;
                if (val != null)
                {
                    DistinctCount(val);
                }
            }
            else if (output is IDictionary<string, object>)
            {
                if((output as IDictionary<string, object>).ContainsKey(Key))
                {
                    DistinctCount((output as IDictionary<string, object>)[Key]);
                }
            }
            else if (output is string && string.IsNullOrEmpty(Key))
            {
                string str = output as string;
                if (_dictionary.ContainsKey(str)) _dictionary[str]++;
                else _dictionary.Add(str, 1);
            }
            else if (output is Int32)
            {
                int n = (int)output;
                if (Type == SummarizationType.Count)
                {
                    DistinctCount(n);
                } else{
                    IntStatistics(n);
                }

            } else if(output is DateTime && string.IsNullOrEmpty(Key))
            {
                DistinctCount(output);
            }
            else
            {
                //object
                if (!string.IsNullOrEmpty(Key))
                {
                    var obj = output.GetType().GetProperty(Key)?.GetValue(output);
                    if (obj != null)
                    {
                        if (Type == SummarizationType.Statistics && obj is Int32) IntStatistics((int)obj);
                        else DistinctCount(obj);
                    }
                }
                else DistinctCount(output);
            }
        }

        public SummarizeCommand()
        {
            _dictionary = new Dictionary<string, int>();
            Interval = SummarizationInterval.Day;
            Type = SummarizationType.Count;
        }
    }

    public enum SummarizationType
    {
        Count,
        Statistics
    }

    public enum SummarizationInterval
    {
        Day,
        Month,
        Year
    }
}

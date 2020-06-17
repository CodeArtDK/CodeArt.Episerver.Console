using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Core
{
    public static class CommandStringParser
    {
        //Double quotes escape's
        public static IEnumerable<CommandElement> PipedCommands(string source)
        {
            CommandElement curElement = new CommandElement();
            //get command name
            int i = 0;
            while(i<source.Length)
            {
                //Get CommandName
                int newI = source.IndexOfAny(new char[] { ' ', '|' }, i);
                if (newI == -1)
                {
                    //Last element, no options
                    curElement.CommandName = source.Substring(i);
                    break;
                }
                curElement.CommandName = source.Substring(i, newI - i);
                i = newI;
                //Get Options, Arguments, piping
                bool inArgument = false;
                bool inOption = false;
                bool inQuotes = false;

                while (i < source.Length)
                {

                

                    if (source[i] == '|')
                    {
                        //Pipe command
                        yield return curElement;
                        curElement = new CommandElement();
                        i++;
                        break;
                    }
                }
            }
            if (curElement.CommandName != null) yield return curElement;

        }
    }

    public class CommandElement
    {
        public string CommandName { get; set; }

        public Dictionary<string,string> Options { get; set; }

        public string Arguments { get; set; }

        public List<string> ArgumentList { get; set; }

        public CommandElement()
        {
            Options = new Dictionary<string, string>();
            ArgumentList = new List<string>();
        }

    }

    
}

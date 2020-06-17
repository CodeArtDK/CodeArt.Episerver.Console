using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command(Keyword ="hello")]
    public class HelloWorldCommand : IConsoleCommand
    {
        //help text?
        //Parameter - maybe attribute or validation? Way to fetch array of all parameters?
        [CommandParameter]
        public string Name { get; set; }

        /// <summary>
        /// Returns a string that will be returned as output.
        /// </summary>
        /// <returns></returns>
        public string Execute(params string[] parameters)
        {
            return "Hello " + Name;
        }
    }
}
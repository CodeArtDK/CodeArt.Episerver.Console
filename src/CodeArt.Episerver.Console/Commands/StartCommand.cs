using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeArt.Episerver.DevConsole.Commands
{

    /// <summary>
    /// Starts a new thread with the commands listed
    /// </summary>
    [Command(Keyword = "start", Description = "Starts following commands as async tasks.", Syntax ="start [regular command line]")]
    public class StartCommand : IConsoleCommand
    {
        public string Execute(params string[] parameters)
        {
            //Will never execute
            return null;
        }
    }
}
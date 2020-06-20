using CodeArt.Episerver.DevConsole.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeArt.Episerver.DevConsole.CLI.Models
{
    public class CLIResponse
    {
        public int LastNo { get; set; }
        public List<CommandLog> LogItems { get; set; }

        public string Session { get; set; }

    }
}

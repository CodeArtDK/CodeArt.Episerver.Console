using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeArt.Episerver.DevConsole.Attributes
{
    public class CommandAttribute : Attribute
    {
        public string Keyword { get; set; }

        public string Syntax { get; set; }

        public string Description { get; set; }

    }
}
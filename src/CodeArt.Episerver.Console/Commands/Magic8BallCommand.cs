using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command(Keyword ="8ball")]
    public class Magic8BallCommand : IConsoleCommand
    {
        public string Execute(params string[] parameters)
        {
            Random r = new Random();
            return new string[] {"Absolutely","I will think about it", "Oh, you know I'll say yes","not right now", "the sources say yes","Doubtful" }[r.Next(6)];
        }
    }
}

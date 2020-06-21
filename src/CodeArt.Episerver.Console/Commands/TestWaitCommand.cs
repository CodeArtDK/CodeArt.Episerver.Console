using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command(Keyword ="testwait")]
    public class TestWaitCommand : IConsoleOutputCommand, IConsoleCommand
    {
        public event OutputToConsoleHandler OutputToConsole;

        [CommandParameter]
        public int WaitTimeBetween { get; set; }

        [CommandParameter]
        public int NumberOfTimes { get; set; }

        [CommandParameter]
        public string Message { get; set; }

        public string Execute(params string[] parameters)
        {
            for(int i = 0; i < NumberOfTimes; i++)
            {
                OutputToConsole?.Invoke(this, string.Format(Message, i));
                Thread.Sleep(WaitTimeBetween);
            }
            return null;
        }

        public TestWaitCommand()
        {
            WaitTimeBetween = 5000;
            NumberOfTimes = 10;
            Message = "Test Wait for the {0}th time";
        }
    }
}

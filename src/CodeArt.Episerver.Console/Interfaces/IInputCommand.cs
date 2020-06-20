using CodeArt.Episerver.DevConsole.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Interfaces
{
    interface IInputCommand : IConsoleCommand
    {
        void Initialize(IOutputCommand Source, params string[] parameters);
    }
}

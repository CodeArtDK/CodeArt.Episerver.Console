﻿using CodeArt.Episerver.DevConsole.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    public class LoadLocalFileCommand : IConsoleCommand, IOutputCommand
    {
        public event CommandOutput OnCommandOutput;

        public string Execute(params string[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}

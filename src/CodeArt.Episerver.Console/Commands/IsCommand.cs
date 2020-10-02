using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using EPiServer.Data.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command("is", Description ="Filters out any piped object that is not assignable as a given type", Syntax ="is [typename] | is not [typename]")]
    public class IsCommand : IConsoleCommand, IInputCommand, IOutputCommand
    {
        public event CommandOutput OnCommandOutput;

        private bool _not;

        private Type _type;

        public string Execute(params string[] parameters)
        {
            return null;
        }

        private static Type ByName(string name)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Reverse())
            {
                var tt = assembly.GetType(name);
                if (tt != null)
                {
                    return tt;
                }
            }
            return null;
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            Source.OnCommandOutput += Source_OnCommandOutput;
            _not = false;
            if (parameters.Length == 2 && parameters[0].ToLower() == "not") _not = true;
            string tpname = parameters.Last().Trim('"');
            _type = Type.GetType(tpname, false, true);
            if (_type == null)
            {
                _type = ByName(tpname);
            }

        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            if (_type == null) return;
            bool isit = _type.IsAssignableFrom(output.GetType());
            bool rt = (_not) ? !isit : isit;
            if (rt) OnCommandOutput?.Invoke(this, output);
        }
    }
}
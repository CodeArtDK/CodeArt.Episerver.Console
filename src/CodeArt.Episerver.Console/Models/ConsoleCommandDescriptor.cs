using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Models
{
    public class ConsoleCommandDescriptor
    {
        public Type CommandType { get; set; }

        public Dictionary<string,PropertyInfo> Parameters { get; set; }

        public CommandAttribute Info { get; set; }

        public void LoadParameters()
        {
            Parameters = new Dictionary<string, PropertyInfo>();

            foreach(var p in CommandType.GetProperties())
            {
                var pa = p.GetCustomAttribute<CommandParameterAttribute>();
                if (pa != null) {
                    if (!string.IsNullOrEmpty(pa.Name)) Parameters.Add(pa.Name.ToLower(), p);
                    else Parameters.Add(p.Name.ToLower(), p);
                }
            }
        }

        public ConsoleCommandDescriptor(Type t)
        {
            this.CommandType = t;
            var ca = CommandType.GetCustomAttributes(typeof(CommandAttribute), true).FirstOrDefault() as CommandAttribute;
            if (ca == null)
            {
                Info = new CommandAttribute() { Keyword = CommandType.Name };
            } else
            {
                Info = ca;
            }

            this.LoadParameters();
        }

        public string Keyword { get { return Info.Keyword; } }

        public T CreateNew<T>() where T:IConsoleCommand
        {
            return (T)ServiceLocator.Current.GetInstance(CommandType);
            //return (T) Activator.CreateInstance(CommandType);
        }
    }
}

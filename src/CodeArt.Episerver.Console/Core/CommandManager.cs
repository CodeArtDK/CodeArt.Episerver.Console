using EPiServer.Framework;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer.Framework.Initialization;
using CodeArt.Episerver.DevConsole.Models;
using EPiServer.Personalization;
using System.Threading;
using CodeArt.Episerver.DevConsole.Interfaces;

namespace CodeArt.Episerver.DevConsole.Core
{
    [ServiceConfiguration(Lifecycle = ServiceInstanceScope.Singleton, ServiceType = typeof(CommandManager))]
    public class CommandManager 
    {

        //New stuff
        public Dictionary<string,ConsoleCommandDescriptor> Commands { get; set; }


        public Dictionary<string,List<CommandLog>> Log { get; set; }

        //Support multiple logs


        public List<CommandJob> ActiveJobs { get; set; }


        private Random _rand = new Random();

        public void UpdateJobs()
        {
            //Logging
            foreach(var j in ActiveJobs)
            {
                CommandLog cl = null;
                while(j.LogQueue.TryDequeue(out cl))
                {
                    //Log.Add(cl);
                }
            }
            //Clean up
            ActiveJobs.RemoveAll(cj => cj.Thread.ThreadState == ThreadState.Stopped);
        }


        protected void CleanUp()
        {
            List<string> toRemove = new List<string>();
            //Clean up old logs
            foreach(var k in Log.Keys)
            {
                if (Log[k].Last().Time > DateTime.Now.AddDays(1)) toRemove.Add(k); 
            }
            foreach (var k in toRemove) Log.Remove(k);
        }

        protected List<CommandLog> NewSession(string sessionID)
        {
            if (Log.ContainsKey(sessionID)) return Log[sessionID];
            var log = new List<CommandLog>();
            Log.Add(sessionID, log);
            log.Add(new CommandLog("System", $"Episerver {typeof(EPiServer.Core.ContentReference).Assembly.GetName().Version.ToString()} loaded and ready."));
            return log;
        }

        protected void AddLogToSession(string session, CommandLog logItem)
        {
            var log = NewSession(session);
            log.Add(logItem);
        }

        public IEnumerable<CommandLog> GetLogs(string session,int Skip)
        {
            var log = NewSession(session);
            return log.Skip(Skip);
        }

        public CommandManager()
        {
            Commands = new Dictionary<string, ConsoleCommandDescriptor>();
            ActiveJobs = new List<CommandJob>();
            Log = new Dictionary<string, List<CommandLog>>();
        }

        public static string[] SplitArguments(string commandLine)
        {
            var parmChars = commandLine.ToCharArray();
            var inSingleQuote = false;
            var inDoubleQuote = false;
            for (var index = 0; index < parmChars.Length; index++)
            {
                if (parmChars[index] == '"' && !inSingleQuote)
                {
                    inDoubleQuote = !inDoubleQuote;
                    parmChars[index] = '\n';
                }
                if (parmChars[index] == '\'' && !inDoubleQuote)
                {
                    inSingleQuote = !inSingleQuote;
                    parmChars[index] = '\n';
                }
                if (!inSingleQuote && !inDoubleQuote && parmChars[index] == ' ')
                    parmChars[index] = '\n';
            }
            return (new string(parmChars)).Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }


        public void ExecuteCommand(string command, string session)
        {
            var commands = command.Split('|').Where(p => p != "").ToArray();

            List<ExecutableCommand> ecommands = new List<ExecutableCommand>();
            foreach (var cmds in commands)
            {
                ExecutableCommand ecmd = new ExecutableCommand();
                
                var parts = SplitArguments(cmds).Where(p => p != "").ToArray(); //TODO: Better arguments handling (support quotes)
                ecmd.Parameters = parts.Skip(1).ToArray();
                //Support piping

                if (Commands.ContainsKey(parts.First().ToLower()))
                {
                    var cmdd = Commands[parts.First().ToLower()];

                    //Create command object
                    var cmd = cmdd.CreateNew<IConsoleCommand>();
                    ecmd.Command = cmd;
                    //Map parameters

                    for (int i = 1; i < parts.Length; i += 2)
                    {
                        if (parts[i].StartsWith("-"))
                        {
                            //Parameter
                            if (cmdd.Parameters.ContainsKey(parts[i].ToLower().TrimStart('-')))
                            {
                                var pi = cmdd.Parameters[parts[i].ToLower().TrimStart('-')];
                                if (pi.PropertyType == typeof(string))
                                {
                                    pi.SetValue(cmd, parts[i + 1]); //TODO: Support other types than string
                                }
                                else if (pi.PropertyType == typeof(bool))
                                {
                                    pi.SetValue(cmd, bool.Parse(parts[i + 1]));
                                }
                                else if(pi.PropertyType == typeof(int))
                                {
                                    pi.SetValue(cmd, int.Parse(parts[i + 1]));
                                } else if (pi.PropertyType.IsEnum)
                                {
                                    pi.SetValue(cmd, Enum.Parse(pi.PropertyType, parts[i + 1]));
                                }
                            } else 
                            {
                                AddLogToSession(session, new CommandLog("System", "Unrecognized parameter"));
                            }
                        }
                        else
                        {
                            //Log.Add("Unknown parameter: " + parts[i]);
                        }
                    }

                }
                else AddLogToSession(session,new CommandLog("System","Unknown Command"));

                if(ecmd.Command is IConsoleOutputCommand)
                {
                    ((IConsoleOutputCommand)ecmd.Command).OutputToConsole += new OutputToConsoleHandler((c, s) => { if (s != null) AddLogToSession(session,new CommandLog(c.GetType().Name,s)); });
                }

                if (ecmd.Command is IInputCommand && ecommands.Any() && (ecommands.Last().Command is IOutputCommand)) (ecmd.Command as IInputCommand).Source = ecommands.Last().Command as IOutputCommand;

                if(ecommands.Count>0 && !(ecmd.Command is IInputCommand))
                {
                    AddLogToSession(session, new CommandLog("System", "You cannot pipe content to that command"));
                    return;
                } else if(commands.Length>1 && ecommands.Count==0 && !(ecmd.Command is IOutputCommand))
                {
                    AddLogToSession(session, new CommandLog("System", "You cannot pipe content from that command"));
                    return;
                }
                ecommands.Add(ecmd);
            }
            //Chain them togetehr
            
            //Execute
            ecommands.Reverse();
            foreach(var ec in ecommands)
            {
                var exec = ec.Command.Execute(ec.Parameters);
                if(!string.IsNullOrEmpty(exec))     AddLogToSession(session, new CommandLog(ec.Command.GetType().Name, exec));

            }

        }

    }
}
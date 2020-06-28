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


        public List<System.Threading.Tasks.Task> Tasks { get; set; }

        private Random _rand = new Random();



        protected void CleanUp()
        {
            List<string> toRemove = new List<string>();
            //Clean up old logs
            foreach(var k in Log.Keys)
            {
                if (Log[k].Last().Time > DateTime.Now.AddDays(1)) toRemove.Add(k); 
            }
            foreach (var k in toRemove) Log.Remove(k);

            //Clean up jobs
            Tasks.RemoveAll(t => t.IsCompleted || t.IsCanceled);
            
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

        public IEnumerable<CommandLog> GetLogs(string session)
        {
            var log = NewSession(session);
            return log;
        }

        public CommandManager()
        {
            Commands = new Dictionary<string, ConsoleCommandDescriptor>();
            Log = new Dictionary<string, List<CommandLog>>();
            Tasks = new List<System.Threading.Tasks.Task>();

            
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
            bool executeasync = false;
            CleanUp();
            if (command.ToLower().StartsWith("start "))
            {
                executeasync = true;
                command = command.Substring(6);
            }
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
                            try
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
                                    else if (pi.PropertyType == typeof(int))
                                    {
                                        pi.SetValue(cmd, int.Parse(parts[i + 1]));
                                    }
                                    else if (pi.PropertyType.IsEnum)
                                    {
                                        pi.SetValue(cmd, Enum.Parse(pi.PropertyType, parts[i + 1], true));
                                    }
                                }
                                else
                                {
                                    AddLogToSession(session, new CommandLog("System", "Unrecognized parameter"));
                                }
                            } catch(Exception exc)
                            {
                                AddLogToSession(session, new CommandLog("System", $"Error in parameters for {cmdd}: {exc.Message}"));
                            }
                        }
                        else
                        {
                            //Log.Add("Unknown parameter: " + parts[i]);
                        }
                    }

                } 
                else AddLogToSession(session,new CommandLog("System",$"Unknown Command: {parts.First()}"));

                if(ecmd.Command is IConsoleOutputCommand)
                {
                    ((IConsoleOutputCommand)ecmd.Command).OutputToConsole += new OutputToConsoleHandler((c, s) => { if (s != null) AddLogToSession(session,new CommandLog(c.GetType().Name,s)); });
                }
                if(ecmd.Command is ILogAwareCommand)
                {
                    ((ILogAwareCommand)ecmd.Command).Log = NewSession(session);
                }

                if (ecmd.Command is IInputCommand && ecommands.Any() && (ecommands.Last().Command is IOutputCommand)) 
                    (ecmd.Command as IInputCommand).Initialize(ecommands.Last().Command as IOutputCommand, ecmd.Parameters);

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


            //Execute

            Action ExecuteCommands = () => {
                foreach (var ec in ecommands)
                {
                    try
                    {
                        var exec = ec.Command.Execute(ec.Parameters);
                        if (!string.IsNullOrEmpty(exec)) AddLogToSession(session, new CommandLog(ec.Command.GetType().Name, exec));

                    }
                    catch (Exception exc)
                    {
                        AddLogToSession(session, new CommandLog(ec.Command.GetType().Name, exc.Message));
                        //TODO: Logging?
                    }
                }
            };

            if (executeasync)
            {
                //Queue task
                AddLogToSession(session, new CommandLog("System", "Starting Task"));
                System.Threading.Tasks.Task t = System.Threading.Tasks.Task.Run(ExecuteCommands);
                Tasks.Add(t);

            }
            else ExecuteCommands();
            

        }

    }
}
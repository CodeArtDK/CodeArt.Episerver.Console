
using CodeArt.Episerver.DevConsole.CLI.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeArt.Episerver.DevConsole.CLI
{
    class Program
    {

        private static string Endpoint;
        private static string Token;

        private static RestClient Client;

        private static string Session;

        static int FetchLatest(int LastLog)
        {
            var q = new RestRequest("FetchLog", Method.GET);
            q.AddQueryParameter("LastLogNo", LastLog.ToString());
            if (Session != null) q.AddQueryParameter("session", Session);
            var r = Client.Execute<CLIResponse>(q);
            var defcolor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Session = r.Data.Session;
            foreach(var s in r.Data.LogItems)
            {
                Console.WriteLine(s);
            }
            Console.ForegroundColor = defcolor;
            return r.Data.LastNo;
        }

        static void RunCommand(string Command)
        {
            var q = new RestRequest("RunCommand", Method.POST);
            q.AddParameter("command", Command);
            q.AddParameter("session", Session);

            if (Command.ToLower().StartsWith("upload"))
            {
                //Handle upload of file
                string fn = Command.Split('|').First().Substring(6).Trim().Trim('"');
                if (string.IsNullOrEmpty(fn))
                {
                    //Ask for filename
                    Console.Write("File to upload: ");
                    fn = Console.ReadLine();
                }
                q.AddParameter("data", Convert.ToBase64String(File.ReadAllBytes(fn)));
                q.AddParameter("filename", Path.GetFileName(fn));
            }

            var r = Client.Execute<dynamic>(q);
            if (Session == null) Session = (string) r.Data.Session;
            if(r.Data.ContainsKey("Filename"))
            {
                string fn = (string) r.Data["Filename"];
                byte[] data = Convert.FromBase64String((string) r.Data["Data"]);
                using (FileStream fs = File.Create(fn))
                {
                    fs.Write(data, 0, data.Length);
                }
                Console.WriteLine("File downloaded: " + fn);
            }
        }

        static void MainLoop()
        {
            string input = null;
            int lastLog = 0;
            CLIResponse resp = new CLIResponse();
            while (input?.ToLower() != "exit")
            {
                //TODO: Support for additional 'local' commands - like loading a file from local and uploading it. Downloading stuff, etc.

                if (!string.IsNullOrEmpty(input))
                {
                    RunCommand(input);
                }
                lastLog = FetchLatest(lastLog);
                Console.Write("command> ");input = Console.ReadLine();
            }
        }

        /// <summary>
        /// Runs the main program
        /// </summary>
        /// <param name="args">Using CommandLine parser. Endpoint and Token needed!</param>
        [MTAThread] 
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Write("Endpoint: ");
                Endpoint = Console.ReadLine();
            } else if(args.Length == 1)
            {
                if (args.First().ToLower() == "-help")
                {
                    Console.WriteLine("Syntax: DevConsoleCLI.exe [endpoint] [access token] [optional command]");
                    return;
                }
                Endpoint = args[0];
                Console.Write("Access Token: ");
                Token = Console.ReadLine();
            } else if (args.Length >= 2)
            {
                Endpoint = args[0];
                Token = args[1];
            }
            Console.WriteLine($"Connecting to {Endpoint} with access token {Token}");
            
            Client = new RestClient(new Uri(new Uri(Endpoint),"CLI/"));
            Client.AddDefaultHeader("Authorization", "Bearer " + Token);

            var req = new RestRequest("Index", Method.GET);
            var resp = Client.Execute(req);
            if (resp.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("Unable to establish contact with endpoint. " + resp.ErrorMessage);
                return;
            }

            if (args.Length > 2)
            {
                //A command is included
                RunCommand(string.Join(" ", args.Skip(2).ToArray()));

                
            } else  MainLoop();

        }
    }
}

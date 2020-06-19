
using CodeArt.Episerver.DevConsole.CLI.Models;
using RestSharp;
using System;
using System.Linq;

namespace CodeArt.Episerver.DevConsole.CLI
{
    class Program
    {

        private static string Endpoint;
        private static string Token;

        private static RestClient Client;


        static int FetchLatest(int LastLog)
        {
            var q = new RestRequest("FetchLog", Method.GET);
            q.AddQueryParameter("LastLogNo", LastLog.ToString());
            var r = Client.Execute<CLIResponse>(q);
            var defcolor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
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
            var r = Client.Execute(q);
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
        [MTAThread] //TODO: Make multithreaded
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Syntax: [Endpoint url] [Access Token]");
                return;
            }
            Endpoint = args[0];
            Token = args[1];

            Client = new RestClient(Endpoint);
            Client.AddDefaultQueryParameter("Token", Token);

            var req = new RestRequest("Index", Method.GET);
            var resp = Client.Execute(req);
            if (resp.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("Unable to establish contact with endpoint. " + resp.ErrorMessage);
                return;
            }
            MainLoop();

        }
    }
}

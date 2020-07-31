using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using CodeArt.Episerver.DevConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command("tofile",Description ="Creates a file from piped input, that can be mailed or downloaded")]
    public class ToFileCommand : IConsoleCommand, IInputCommand, IOutputCommand
    {
        public event CommandOutput OnCommandOutput;

        [CommandParameter]
        public string FileName { get; set; }

        [CommandParameter]
        public string Mimetype { get; set; }


        public TransferFile File { get; private set; }


        protected List<string> contents { get; set; }

        private string GenerateFileName()
        {
            Random r = new Random();
            string rt = "";
            for (int i = 0; i < 6; i++)
                rt += "abcdefghijklmnopqrstuvwxyz"[r.Next(26)];
            return rt + ".txt";
        }


        public string Execute(params string[] parameters)
        {
            if (File == null)
            {
                if (contents == null)
                {
                    //Prepare file for download
                    File = new TransferFile();
                    File.FileName = FileName ?? "test.txt";
                    File.Mimetype = Mimetype ?? "text/plain";
                    File.Data = UTF8Encoding.UTF8.GetBytes("This is a test text file");
                    return "No file provided as input. Test file generated.";
                }
                else
                {
                    File = new TransferFile();
                    File.FileName = FileName ?? GenerateFileName();
                    File.Mimetype = Mimetype ?? "text/plain";
                    File.Data = UTF8Encoding.UTF8.GetBytes(string.Join("\n", contents.ToArray()));

                }
                OnCommandOutput?.Invoke(this, File);
            }

            return null;
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            if (string.IsNullOrEmpty(FileName) && parameters.Length > 0)
                FileName = parameters.First();
            if (string.IsNullOrEmpty(Mimetype) && parameters.Length == 2)
                Mimetype = parameters[1];
            else if (string.IsNullOrEmpty(Mimetype) && !string.IsNullOrEmpty(FileName))
            {
                Mimetype = MimeMapping.GetMimeMapping(FileName);
            }
            Source.OnCommandOutput += Source_OnCommandOutput;
        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            if (output is string)
            {
                if (contents == null) contents = new List<string>();
                contents.Add(output as string);
            } else if(output is IDictionary<string, object>)
            {
                //Check for contents and create attachment
                var dic = output as IDictionary<string, object>;
                if (dic.ContainsKey("Contents"))
                {
                    var content = (dic["Contents"] is List<string>) ? string.Join("\n", (dic["Contents"] as List<string>).ToArray()) : dic["Contents"] as string;
                    var file = new TransferFile();
                    file.FileName = FileName ?? GenerateFileName();
                    file.Mimetype = Mimetype ?? "text/plain";
                    file.Data = UTF8Encoding.UTF8.GetBytes(content);
                    dic["Attachment"] = file;
                    OnCommandOutput?.Invoke(this, dic);
                }
            }
        }
    }
}

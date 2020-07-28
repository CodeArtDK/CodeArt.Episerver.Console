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
    [Command("upload", Description = "Uploads a local file and passes it on in the command pipeline. Can only be used as the first command.")]
    public class UploadCommand : IOutputCommand, IConsoleCommand
    {
        public event CommandOutput OnCommandOutput;

        public string Execute(params string[] parameters)
        {
            if (HttpContext.Current.Request["filename"] != null && HttpContext.Current.Request["data"]!=null)
            {
                //We have an uploaded file
                TransferFile tf = new TransferFile();
                tf.FileName = HttpContext.Current.Request["filename"];
                tf.Data = Convert.FromBase64String(HttpContext.Current.Request["data"]);
                OnCommandOutput?.Invoke(this, tf);
                return "File uploaded successfully";
            }
            else return "No file uploaded";
            
        }
    }
}

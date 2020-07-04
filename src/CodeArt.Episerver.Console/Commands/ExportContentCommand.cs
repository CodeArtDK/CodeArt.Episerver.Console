using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using CodeArt.Episerver.DevConsole.Models;
using EPiServer.Core;
using EPiServer.Core.Transfer;
using EPiServer.Enterprise;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command("exportcontent",Description ="Creates an export package with the content provided")]
    public class ExportContentCommand : IConsoleCommand, IInputCommand, IReturnsFile
    {

        private readonly IDataExporter _exporter;

        private List<ContentReference> _contentList;

        //TODO: Options, Support categories, visitor groups, ...

        [CommandParameter]
        public bool Recursive { get; set; }

        public DownloadFile File { get; private set; }

        public ExportContentCommand(IDataExporter exporter)
        {
            _exporter = exporter;
            _contentList = new List<ContentReference>();
        }

        public string Execute(params string[] parameters)
        {
                MemoryStream ms = new MemoryStream(4000);

                _exporter.Export(ms, _contentList.Select(c => new ExportSource(c, (Recursive) ? ExportSource.RecursiveLevelInfinity : ExportSource.NonRecursive)).ToList());
                while (!_exporter.Status.IsDone) Thread.Sleep(1000);

                File = new DownloadFile();
                File.FileName = "Export.episerverdata";
                File.Mimetype = "application/octet-stream";
                File.Data = ms.GetBuffer();
                
                return "Content exported";
            
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            Source.OnCommandOutput += Source_OnCommandOutput;
        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            if (output is ContentReference) _contentList.Add(output as ContentReference);
            else if (output is IContent) _contentList.Add((output as IContent).ContentLink);
            else if (output is string) _contentList.Add(ContentReference.Parse(output as string));
            //Support ExportSource input?!
        }
    }
}

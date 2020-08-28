using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command("contentmodelusage",Description ="Lists where the content model is used")]
    public class ContentModelUsageCommand : IConsoleCommand, IOutputCommand, IInputCommand
    {
        private readonly IContentModelUsage _modelUsage;
        private readonly IContentTypeRepository _typeRepository;


        [CommandParameter]
        public string ContentTypeName { get; set; }

        [CommandParameter]
        public bool ListContent { get; set; }

        [CommandParameter]
        public bool UseDictionary { get; set; }


        public ContentModelUsageCommand(IContentModelUsage modelUsage, IContentTypeRepository typeRepository)
        {
            _modelUsage = modelUsage;
            _typeRepository = typeRepository;
            UseDictionary = false;
            ListContent = false;
        }

        public event CommandOutput OnCommandOutput;


        private void HandleContentType(ContentType ct)
        {
            if (!ListContent)
            {
                if (!UseDictionary)
                {
                    OnCommandOutput.Invoke(this, _modelUsage.IsContentTypeUsed(ct));
                }
                else
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("ContentType", ct);
                    dic.Add("IsContentTypeUsed", _modelUsage.IsContentTypeUsed(ct));
                    OnCommandOutput.Invoke(this, dic);
                }

            }
            else
            {
                if (!UseDictionary)
                {
                    foreach (var c in _modelUsage.ListContentOfContentType(ct)) OnCommandOutput.Invoke(this, c);
                }
                else
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("ContentType", ct);
                    dic.Add("ContentUsage", _modelUsage.ListContentOfContentType(ct));
                    OnCommandOutput.Invoke(this, dic);
                }
            }
        }

        public string Execute(params string[] parameters)
        {
            if (string.IsNullOrEmpty(ContentTypeName) && parameters.Length > 0) ContentTypeName = parameters.First();
            if (string.IsNullOrEmpty(ContentTypeName)) return null;
            var ct = _typeRepository.Load(ContentTypeName);
            HandleContentType(ct);
            return null;
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            Source.OnCommandOutput += Source_OnCommandOutput;
        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            if (output is ContentType) HandleContentType((output as ContentType));
            else if (output is string) HandleContentType(_typeRepository.Load(output as string));
            else if (output is int) HandleContentType(_typeRepository.Load((int)output));
        }
    }
}

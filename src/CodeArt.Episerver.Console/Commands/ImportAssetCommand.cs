using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.Framework.Blobs;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command(Keyword = "import-asset", Description = "Import an asset - for example from a URL")]
    public class ImportAssetCommand : IConsoleCommand, IInputCommand, IConsoleOutputCommand, IOutputCommand
    {
        [CommandParameter]
        public string Url { get; set; }

        [CommandParameter(Description ="The path below the Global Asset folder to download to.")]
        public string Destination { get; set; }

        public IOutputCommand Source { get; set; }

        public event OutputToConsoleHandler OutputToConsole;
        public event CommandOutput OnCommandOutput;

        protected Injected<IContentTypeRepository> _trepo { get; set; }
        protected Injected<IContentRepository> _crepo { get; set; }

        protected Injected<ContentMediaResolver> _mresolver { get; set; }

        protected Injected<IBlobFactory> _blobFactory { get; set; }

        protected Injected<ContentAssetHelper> _contentAssetHelper { get; set; }

        protected string DownloadAsset(string url)
        {
            try
            {
                WebClient wc = new WebClient();
                byte[] asset = wc.DownloadData(url);

                string ext = Path.GetExtension(url);
                var ctype = _mresolver.Service.GetFirstMatching(ext);
                ContentType contentType = _trepo.Service.Load(ctype);

                //TODO: Support destination that should be resolved.
                var assetFile = _crepo.Service.GetDefault<MediaData>(SiteDefinition.Current.GlobalAssetsRoot, contentType.ID);
                assetFile.Name = Path.GetFileName(url);

                var blob = _blobFactory.Service.CreateBlob(assetFile.BinaryDataContainer, ext);
                using (var s = blob.OpenWrite())
                {
                    var w = new StreamWriter(s);
                    w.BaseStream.Write(asset, 0, asset.Length);
                    w.Flush();
                }
                assetFile.BinaryData = blob;
                var assetContentRef = _crepo.Service.Save(assetFile, SaveAction.Publish);

                OnCommandOutput?.Invoke(this, assetContentRef); //If piped, output the reference that was just created
            } catch(Exception exc)
            {
                return exc.Message;
            }

            return null;
        }

        public string Execute(params string[] parameters)
        {
            if (Source != null) Source.OnCommandOutput += Source_OnCommandOutput;
            if (!string.IsNullOrEmpty(Url))
            {
                string success=DownloadAsset(Url);
                if (success == null) return $"Successfully downloaded asset {Url}";
                else return $"Failed to download asset {Url} with error: {success}";
            }
            return null;
        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            //accepts strings with urls
            
            string success = DownloadAsset((string)output);
            if (success == null) OutputToConsole?.Invoke(this, $"Successfully downloaded asset {(string)output}");
            else OutputToConsole?.Invoke(this, $"Failed to download asset {Url} with error: {success}");

        }
    }
}

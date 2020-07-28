using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Models
{
    public class TransferFile
    {
        public string FileName { get; set; }

        public string Mimetype { get; set; }

        public byte[] Data { get; set; }

    }
}

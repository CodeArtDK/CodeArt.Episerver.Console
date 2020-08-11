using CodeArt.Episerver.DevConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Interfaces
{
    public interface ILogAwareCommand
    {
        List<CommandLog> Log { get; set; }
    }
}

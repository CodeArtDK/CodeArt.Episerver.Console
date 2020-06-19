using EPiServer.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CodeArt.Episerver.DevConsole.Core
{
    public class CLIUserRole : VirtualRoleProviderBase
    {
        public override bool IsInVirtualRole(IPrincipal principal, object context)
        {
            if (HttpContext.Current.Request["token"] == "test") return true; //TODO: Make optional header that looks up an Access token in a database.

            else return false;
        }
    }
}

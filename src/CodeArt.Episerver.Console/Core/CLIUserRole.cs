using CodeArt.Episerver.DevConsole.AccessTokens;
using EPiServer.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            var header = HttpContext.Current.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(header)) return false;
            if (!header.StartsWith("Bearer ")) return false;
            string token = header.Substring(7);
            AccessTokenStore store = new AccessTokenStore();
            var at=store.LoadToken(token);
            if (at == null) return false;
            else return true;
        }
    }
}

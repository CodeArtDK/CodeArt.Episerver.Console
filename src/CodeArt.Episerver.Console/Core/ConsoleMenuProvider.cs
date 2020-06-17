using EPiServer.Security;
using EPiServer.Shell;
using EPiServer.Shell.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.Core
{
    [MenuProvider]
    public class ConsoleMenuProvider : IMenuProvider
    {
        const string ModuleName = "CodeArt.Episerver.Console";

        const string GlobalMenuTitle = "Developer";
        const string GlobalMenuLogicalPath = "/global/DeveloperTools";

        const string ConsoleTitle = "Console";
        const string ConsolePath = "global/DeveloperTools/Console";


        public IEnumerable<MenuItem> GetMenuItems()
        {        
            var developerSection = new SectionMenuItem(GlobalMenuTitle, GlobalMenuLogicalPath)
            {
                IsAvailable = request => PrincipalInfo.HasAdminAccess
            };
            var console = CreateUrlMenuItem(ConsoleTitle, ConsolePath, Paths.ToResource(ModuleName, "Console"));

            yield return developerSection;
            yield return console;
        }

        protected virtual UrlMenuItem CreateUrlMenuItem(string title, string logicalPath, string resourcePath)
        {
            return new UrlMenuItem(title, logicalPath, resourcePath)
            {
                IsAvailable = request => PrincipalInfo.HasAdminAccess
            };
        }
    }
}

using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.AccessTokens
{
    [Command(Keyword = "createaccesstoken", Group = CommandGroups.ACCESSTOKENS, Description ="Create a new access token, to use with the CLI.")]
    public class CreateAccessTokenCommand : IConsoleCommand
    {
        [CommandParameter]
        public bool Impersonate { get; set; }

        public string Execute(params string[] parameters)
        {
            AccessTokenStore store = new AccessTokenStore();
            var token = store.CreateToken(Impersonate);

            return "Access token created. Use this token: " + token;
        }
    }
}

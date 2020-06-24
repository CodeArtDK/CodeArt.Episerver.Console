using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.AccessTokens
{
    [Command(Keyword = "removeaccesstoken", Group = CommandGroups.ACCESSTOKENS, Description = "Removes an access token")]
    public class RemoveAccessTokenCommand : IConsoleCommand
    {

        [CommandParameter]
        public string TokenId { get; set; }

        public string Execute(params string[] parameters)
        {
            if (string.IsNullOrEmpty(TokenId) && parameters.Length == 1) TokenId = parameters.First();
            else if (string.IsNullOrEmpty(TokenId)) return "You have to provide a token id";

            AccessTokenStore store = new AccessTokenStore();
            store.RemoveToken(TokenId);

            return "Access token removed";
        }
    }
}
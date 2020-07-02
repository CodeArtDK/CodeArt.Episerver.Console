using CodeArt.Episerver.DevConsole.Attributes;
using CodeArt.Episerver.DevConsole.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace CodeArt.Episerver.DevConsole.Commands
{
    [Command(Keyword ="listusers",Description ="List users for the pipe")]
    public class ListUsersCommand : IConsoleCommand, IOutputCommand
    {

        [CommandParameter]
        public bool OnlyOnline { get; set; }

        [CommandParameter]
        public string ByEmail { get; set; }

        [CommandParameter]
        public string ByName { get; set; }


        public event CommandOutput OnCommandOutput;

        public string Execute(params string[] parameters)
        {
            MembershipUserCollection results = null;
            if (!string.IsNullOrEmpty(ByEmail))
            {
                results = Membership.FindUsersByEmail(ByEmail);
            }
            else if (!string.IsNullOrEmpty(ByName))
            {
                results = Membership.FindUsersByName(ByName);
            }
            else results = Membership.GetAllUsers();
            int usercnt = 0;
            foreach(MembershipUser user in results)
            {
                usercnt++;
                if (OnlyOnline && !user.IsOnline) continue;

                OnCommandOutput?.Invoke(this, user);
            }
            int onlinecnt = Membership.GetNumberOfUsersOnline();
            return $"Went through {usercnt} users. {onlinecnt} users online.";
        }
    }
}

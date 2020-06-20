using EPiServer.Data;
using EPiServer.Data.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.AccessTokens
{
    [EPiServerDataStore(AutomaticallyCreateStore = true, AutomaticallyRemapStore = true)]

    public class AccessToken : IDynamicData
    {

        //Created
        public DateTime Created { get; set; }

        //CreatedBy
        public string Owner { get; set; }

        //Token hash (Base64)
        [EPiServerDataIndex]
        public string TokenHash { get; set; }

        //Impersonate?
        public bool Impersonate { get; set; }

        public Identity Id { get; set; }
    }
}

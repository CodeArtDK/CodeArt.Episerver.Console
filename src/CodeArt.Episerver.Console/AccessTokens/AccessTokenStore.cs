using EPiServer.Data.Dynamic;
using EPiServer.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Episerver.DevConsole.AccessTokens
{
    public class AccessTokenStore
    {
        protected DynamicDataStore store { get; set; }
        public AccessTokenStore()
        {
            store = typeof(AccessToken).GetOrCreateStore();

        }

        public AccessToken LoadToken(string token)
        {
            var bytes = Convert.FromBase64String(token);
            SHA512 shaM = new SHA512Managed();
            var hashbytes = shaM.ComputeHash(bytes);
            string hash = Convert.ToBase64String(hashbytes);
            var accesstoken = store.Items<AccessToken>().Where(at => at.TokenHash == hash).FirstOrDefault();
            return accesstoken;
        }

        public void RemoveToken(AccessToken token)
        {
            store.Delete(token.Id);
        }

        public void RemoveToken(string Id)
        {
            store.Delete(Id);
        }


        public IEnumerable<AccessToken> ListTokens(bool ShowAll)
        {
            if (ShowAll) return store.Items<AccessToken>();
            else return store.Items<AccessToken>().Where(at => at.Owner == PrincipalInfo.Current.Name);
        }

        public string CreateToken(bool Impersonate)
        {
            var srcToken = Guid.NewGuid();
            AccessToken at = new AccessToken();
            at.Owner = PrincipalInfo.Current.Name;
            at.Impersonate = Impersonate;
            at.Created = DateTime.Now;
            SHA512 shaM = new SHA512Managed();
            var hashbytes=shaM.ComputeHash(srcToken.ToByteArray());
            at.TokenHash = Convert.ToBase64String(hashbytes);
            store.Save(at);

            return System.Convert.ToBase64String(srcToken.ToByteArray());
        }
    }
}

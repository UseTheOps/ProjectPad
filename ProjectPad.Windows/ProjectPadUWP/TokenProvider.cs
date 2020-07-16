using ProjectPad.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using System.IO;
using Newtonsoft.Json;

namespace ProjectPadUWP
{
    class TokenProvider : ITokenProvider
    {

        private class Secrets
        {
            public Msalsettings MSALSettings { get; set; }
        }

        private class Msalsettings
        {
            public string clientId { get; set; }
        }


        public static IPublicClientApplication PublicClientApp;


        protected void OnTokenChanged()
        {
            TokenChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler TokenChanged;

        static TokenProvider()
        {
            string clientId = null;
            using (var rdr = new StreamReader(typeof(TokenProvider).Assembly.GetManifestResourceStream("ProjectPadUWP.secrets.json")))
            {
                var settings = JsonConvert.DeserializeObject<Secrets>(rdr.ReadToEnd());
                clientId = settings?.MSALSettings?.clientId;
            }

            if (string.IsNullOrEmpty(clientId))
                throw new ApplicationException("The secrets.json file is missing, please read 'How to build' section in readme");

            PublicClientApp = PublicClientApplicationBuilder.Create(clientId)
                    .WithRedirectUri("https://login.microsoftonline.com/common/oauth2/nativeclient")
                    .Build();
        }

        public async Task<string> GetToken()
        {
            var accounts = await PublicClientApp.GetAccountsAsync();
            if (accounts != null)
            {
                var firstAccount = accounts.FirstOrDefault();
                if (firstAccount != null)
                {
                    var authResult = await PublicClientApp.AcquireTokenSilent(null, firstAccount)
                                                          .ExecuteAsync();
                    if (authResult != null)
                        return authResult.AccessToken;
                }
            }

            var authResult2 = await PublicClientApp.AcquireTokenInteractive(null)
                      .ExecuteAsync();
            if (authResult2 != null)
            {
                OnTokenChanged();
                return authResult2.AccessToken;
            }

            return null;
        }

        public async Task<bool> HasSilentToken()
        {
            var accounts = await PublicClientApp.GetAccountsAsync();
            if (accounts == null)
                return false;
            var firstAccount = accounts.FirstOrDefault();
            if (firstAccount == null)
                return false;
            var authResult = await PublicClientApp.AcquireTokenSilent(null, firstAccount)
                                                  .ExecuteAsync();
            if (authResult == null)
                return false;
            return true;
        }

        public async Task ClearAllTokens()
        {
            var accounts = await PublicClientApp.GetAccountsAsync();
            if(accounts!=null && accounts.Count()>0)
            {
                foreach(var acc in accounts)
                    await PublicClientApp.RemoveAsync(acc);
                OnTokenChanged();
            }
        }
    }
}

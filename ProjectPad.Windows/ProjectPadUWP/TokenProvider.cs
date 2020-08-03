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
        private static readonly string[] _graphApiScopes = new string[]
        {
            "https://graph.microsoft.com/email",
            "https://graph.microsoft.com/Files.ReadWrite.AppFolder",
            "https://graph.microsoft.com/offline_access",
            "https://graph.microsoft.com/User.Read",
            "https://graph.microsoft.com/Files.ReadWrite.All"
        };

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
                var settings = JsonConvert.DeserializeObject<TokenFileSecretsData>(rdr.ReadToEnd());
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
            try
            {
                var accounts = await PublicClientApp.GetAccountsAsync();
                if (accounts != null)
                {
                    var firstAccount = accounts.FirstOrDefault();
                    if (firstAccount != null)
                    {
                        try
                        {
                            var authResult = await PublicClientApp.AcquireTokenSilent(_graphApiScopes, firstAccount)
                                                                  .ExecuteAsync();
                            if (authResult != null)
                                return authResult.AccessToken;
                        }
                        catch (MsalUiRequiredException)
                        {

                        }
                    }
                }

                var authResult2 = await PublicClientApp.AcquireTokenInteractive(_graphApiScopes)
                          .ExecuteAsync();
                if (authResult2 != null)
                {
                    OnTokenChanged();
                    return authResult2.AccessToken;
                }
            }
            catch (MsalException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

            return null;
        }

        public async Task<bool> HasSilentToken()
        {
            try
            {
                var accounts = await PublicClientApp.GetAccountsAsync();
                if (accounts == null)
                    return false;
                var firstAccount = accounts.FirstOrDefault();
                if (firstAccount == null)
                    return false;
                var authResult = await PublicClientApp.AcquireTokenSilent(_graphApiScopes, firstAccount)
                                                      .ExecuteAsync();
                if (authResult == null)
                    return false;
                return true;

            }
            catch (MsalUiRequiredException)
            {
                return false;
            }
        }

        public async Task ClearToken()
        {
            var accounts = await PublicClientApp.GetAccountsAsync();
            if (accounts != null && accounts.Count() > 0)
            {
                foreach (var acc in accounts)
                    await PublicClientApp.RemoveAsync(acc);
                OnTokenChanged();
            }
        }
    }


    class TokenFileSecretsData
    {
        public Msalsettings MSALSettings { get; set; }
        public Msalsettings ADOSettings { get; set; }
    }

    class Msalsettings
    {
        public string clientId { get; set; }
    }

    class Adosettings
    {
        public string clientId { get; set; }
    }

    class AzureDevOpsTokenProvider : ITokenProvider
    {
        private static readonly string[] _graphApiScopes = new string[]
       {
            "https://app.vssps.visualstudio.com/user_impersonation"
       };




        public static IPublicClientApplication PublicClientApp;


        protected void OnTokenChanged()
        {
            TokenChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler TokenChanged;

        static AzureDevOpsTokenProvider()
        {
            string clientId = null;
            using (var rdr = new StreamReader(typeof(TokenProvider).Assembly.GetManifestResourceStream("ProjectPadUWP.secrets.json")))
            {
                var settings = JsonConvert.DeserializeObject<TokenFileSecretsData>(rdr.ReadToEnd());
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
            try
            {
                var accounts = await PublicClientApp.GetAccountsAsync();
                if (accounts != null)
                {
                    var firstAccount = accounts.FirstOrDefault();
                    if (firstAccount != null)
                    {
                        try
                        {
                            var authResult = await PublicClientApp.AcquireTokenSilent(_graphApiScopes, firstAccount)
                                                                  .ExecuteAsync();
                            if (authResult != null)
                                return authResult.AccessToken;
                        }
                        catch (MsalUiRequiredException)
                        {

                        }
                    }
                }

                var authResult2 = await PublicClientApp.AcquireTokenInteractive(_graphApiScopes)
                          .ExecuteAsync();
                if (authResult2 != null)
                {
                    OnTokenChanged();
                    return authResult2.AccessToken;
                }
            }
            catch (MsalException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

            return null;
        }

        public async Task<bool> HasSilentToken()
        {
            try
            {
                var accounts = await PublicClientApp.GetAccountsAsync();
                if (accounts == null)
                    return false;
                var firstAccount = accounts.FirstOrDefault();
                if (firstAccount == null)
                    return false;
                var authResult = await PublicClientApp.AcquireTokenSilent(_graphApiScopes, firstAccount)
                                                      .ExecuteAsync();
                if (authResult == null)
                    return false;
                return true;

            }
            catch (MsalUiRequiredException)
            {
                return false;
            }
        }

        public async Task ClearToken()
        {
            var accounts = await PublicClientApp.GetAccountsAsync();
            if (accounts != null && accounts.Count() > 0)
            {
                foreach (var acc in accounts)
                    await PublicClientApp.RemoveAsync(acc);
                OnTokenChanged();
            }
        }
    }
}

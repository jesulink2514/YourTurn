using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace Techies.YourTurn.Security
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AzureADB2COptions _options;
        private readonly IPublicClientApplication _pca;

        public AuthenticationService(
            AzureADB2COptions options,
            IParentWindowLocator windowLocator)
        {
            _options = options;


            var builder = PublicClientApplicationBuilder.Create(options.ClientId);

            builder = builder.WithRedirectUri($"msal{options.ClientId}://auth")
            .WithIosKeychainSecurityGroup(options.iOSKeyChainGroup)
            .WithB2CAuthority($"{_options.AuthorityBase}{_options.PolicySignUpSignIn}");

            if (Device.RuntimePlatform == "Android")
            {
                builder = builder.WithParentActivityOrWindow(windowLocator.GetCurrentWindow);
            }

            _pca = builder.Build();
        }

        public async Task<UserContext> SignInAsync()
        {
            UserContext newContext;
            try
            {
                // acquire token silent
                newContext = await AcquireTokenSilent();
            }
            catch (MsalUiRequiredException)
            {
                // acquire token interactive
                newContext = await SignInInteractively();
            }
            return newContext;
        }

        public async Task<UserContext> SignOutAsync()
        {

            var accounts = (await _pca.GetAccountsAsync()).ToArray();
            while (accounts.Any())
            {
                await _pca.RemoveAsync(accounts.FirstOrDefault());
                accounts = (await _pca.GetAccountsAsync()).ToArray();
            }

            var signedOutContext = new UserContext {IsLoggedOn = false};
            return signedOutContext;
        }

        private async Task<UserContext> AcquireTokenSilent()
        {
            var accounts = await _pca.GetAccountsAsync().ConfigureAwait(false);

            var authResult = await _pca.AcquireTokenSilent(_options.Scopes,
                    GetAccountByPolicy(accounts, _options.PolicySignUpSignIn)
                )
               .WithB2CAuthority($"{_options.AuthorityBase}{_options.PolicySignUpSignIn}")
               .ExecuteAsync().ConfigureAwait(false);

            return CreateContextFromAuthResult(authResult);
        }

        private UserContext CreateContextFromAuthResult(AuthenticationResult authResult)
        {
            var user = ParseToken(authResult.IdToken);
            var newContext = new UserContext
            {
                IsLoggedOn = true,
                AccessToken = authResult.AccessToken,
                Name = user["given_name"]?.ToString(),
                Surname = user["surname"]?.ToString(),
                DisplayName = user["name"]?.ToString(),
                UserIdentifier = user["oid"]?.ToString(),
                Country = user["country"]?.ToString()
            };

            if (user["emails"] is JArray emails)
            {
                newContext.EmailAddress = emails[0].ToString();
            }

            return newContext;
        }

        private async Task<UserContext> SignInInteractively()
        {
            try
            {
                var accounts = await _pca.GetAccountsAsync();

                var authResult = await _pca.AcquireTokenInteractive(_options.Scopes)
                    .WithAccount(GetAccountByPolicy(accounts, _options.PolicySignUpSignIn))
                    .ExecuteAsync();

                return CreateContextFromAuthResult(authResult);
            }
            catch (MsalClientException)
            {
                return new UserContext { IsLoggedOn = false };
            }
        }

        private IAccount GetAccountByPolicy(IEnumerable<IAccount> accounts, string policy)
        {
            foreach (var account in accounts)
            {
                string userIdentifier = account.HomeAccountId.ObjectId.Split('.')[0];
                if (userIdentifier.EndsWith(policy.ToLower())) return account;
            }

            return null;
        }

        JObject ParseToken(string idToken)
        {
            // Get the piece with actual user info
            idToken = idToken.Split('.')[1];
            idToken = Base64UrlDecode(idToken);
            return JObject.Parse(idToken);
        }

        private string Base64UrlDecode(string s)
        {
            s = s.Replace('-', '+').Replace('_', '/');
            s = s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');
            var byteArray = Convert.FromBase64String(s);
            var decoded = Encoding.UTF8.GetString(byteArray, 0, byteArray.Count());
            return decoded;
        }

    }

    public class UserContext
    {
        public string UserIdentifier { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; internal set; }
        public string Surname { get; set; }
        public string Country { get; internal set; }
        public string EmailAddress { get; internal set; }
        public bool IsLoggedOn { get; internal set; }
        public string AccessToken { get; internal set; }
    }
}

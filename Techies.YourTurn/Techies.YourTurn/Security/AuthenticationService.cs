using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
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

            IEnumerable<IAccount> accounts = await _pca.GetAccountsAsync();
            while (accounts.Any())
            {
                await _pca.RemoveAsync(accounts.FirstOrDefault());
                accounts = await _pca.GetAccountsAsync();
            }
            var signedOutContext = new UserContext();
            signedOutContext.IsLoggedOn = false;
            return signedOutContext;
        }

        private async Task<UserContext> AcquireTokenSilent()
        {
            var accounts = await _pca.GetAccountsAsync();

            var authResult = await _pca.AcquireTokenSilent(_options.Scopes,
                    GetAccountByPolicy(accounts, _options.PolicySignUpSignIn)
                )
               .WithB2CAuthority($"{_options.AuthorityBase}{_options.PolicySignUpSignIn}")
               .ExecuteAsync();

            var newContext = new UserContext
            {
                AccessToken = authResult.AccessToken
            };

            return newContext;
        }

        private async Task<UserContext> SignInInteractively()
        {
            var accounts = await _pca.GetAccountsAsync();

            var authResult = await _pca.AcquireTokenInteractive(_options.Scopes)
                .WithAccount(GetAccountByPolicy(accounts, _options.PolicySignUpSignIn))
                .ExecuteAsync();

            return new UserContext
            {
                AccessToken = authResult.AccessToken
            };
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
    }

    public class UserContext
    {
        public string Name { get; internal set; }
        public string UserIdentifier { get; internal set; }
        public string Country { get; internal set; }
        public string EmailAddress { get; internal set; }
        public bool IsLoggedOn { get; internal set; }
        public string AccessToken { get; internal set; }
    }
}

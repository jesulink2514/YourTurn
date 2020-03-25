using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Identity.Client;
using Xamarin.Forms;

namespace Techies.YourTurn.Security
{
    public class AzureADB2COptions
    {
        public string Tenant { get; set; }
        public string B2CHostName { get; set; }
        public string ClientId { get; set; }
        public string PolicySignUpSignIn { get; set; }
        public string PolicyResetPassword { get; set; }
        public string[] Scopes { get;set; }
        public string AuthorityBase { get; set; }
        public string iOSKeyChainGroup { get; set; }
    }
    public class AuthenticationService
    {
        private readonly AzureADB2COptions _options;
        private readonly IParentWindowLocator _windowLocator;
        private IPublicClientApplication _pca;

        public AuthenticationService(
            AzureADB2COptions options,
            IParentWindowLocator windowLocator)
        {
            _options = options;
            _windowLocator = windowLocator;

            var builder = PublicClientApplicationBuilder.Create(options.ClientId)
                .WithB2CAuthority(options.B2CHostName)
                .WithRedirectUri($"msal{options.ClientId}://auth")
                .WithIosKeychainSecurityGroup(options.iOSKeyChainGroup);

            if (Device.RuntimePlatform == "Android")
            {
                builder = builder.WithParentActivityOrWindow(() => _windowLocator.GetCurrentWindow());
            }

            _pca = builder.Build();
        }
    }

    public interface IParentWindowLocator
    {
        object GetCurrentWindow();
    }
}

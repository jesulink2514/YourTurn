using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Identity.Client;

namespace Techies.YourTurn.ViewModels
{
    public class LoginPageViewModel : BindableBase
    {
        public LoginPageViewModel()
        {
            var builder = PublicClientApplicationBuilder.Create("")
                .WithB2CAuthority("")
                .WithIosKeychainSecurityGroup("")
                .WithRedirectUri($"msal{"xxx"}://auth");

            var app = builder.Build();
        }
    }
}

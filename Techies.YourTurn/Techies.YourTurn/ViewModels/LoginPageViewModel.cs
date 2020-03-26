using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Acr.UserDialogs;
using Microsoft.Identity.Client;
using Techies.YourTurn.Security;

namespace Techies.YourTurn.ViewModels
{
    public class LoginPageViewModel : BindableBase
    {
        private readonly IAuthenticationService _authenticationService;

        public LoginPageViewModel(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
            LoginCommand = new DelegateCommand(OnLogin);
            LogoutCommand = new DelegateCommand(OnLogout);
        }

        public ICommand LoginCommand { get; private set; }
        public ICommand LogoutCommand { get; private set; }

        private async void OnLogin()
        {
            using (UserDialogs.Instance.Loading())
            {
                var result = await _authenticationService.SignInAsync();
                Debug.WriteLine(result.AccessToken);
            }
        }
        private async void OnLogout()
        {
            await _authenticationService.SignOutAsync();
        }
    }
}

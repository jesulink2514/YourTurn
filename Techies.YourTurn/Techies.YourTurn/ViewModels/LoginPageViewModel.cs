using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
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
        }

        public ICommand LoginCommand { get; private set; }

        private async void OnLogin()
        {
            await _authenticationService.SignInAsync();
        }
    }
}

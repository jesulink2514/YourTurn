﻿using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Acr.UserDialogs;
using Microsoft.Identity.Client;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Techies.YourTurn.Security;

namespace Techies.YourTurn.ViewModels
{
    public class LoginPageViewModel : BindableBase, INavigatedAware
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;

        public LoginPageViewModel(
            IAuthenticationService authenticationService,
            IDialogService dialogService,
            INavigationService navigationService)
        {
            _authenticationService = authenticationService;
            _dialogService = dialogService;
            _navigationService = navigationService;
        }


        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public async void OnNavigatedTo(INavigationParameters parameters)
        {
            using (UserDialogs.Instance.Loading("Verifying your account..."))
            {
                var result = await _authenticationService.SignInAsync();
                if (result.IsLoggedOn)
                {
                    await _navigationService.NavigateAsync("MainPage",new NavigationParameters{{"User", result }});
                }
                else
                {
                    await _navigationService.NavigateAsync("/LoginPage");
                }
            }
        }
    }
}

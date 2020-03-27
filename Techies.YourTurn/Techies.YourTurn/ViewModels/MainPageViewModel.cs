using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Acr.UserDialogs;
using Techies.YourTurn.Security;

namespace Techies.YourTurn.ViewModels
{
    public class MainPageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly IAuthenticationService _authenticationService;

        public MainPageViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService)
            : base(navigationService)
        {
            _authenticationService = authenticationService;
            Title = "Main Page";
            LogoutCommand = new DelegateCommand(OnLogout);
        }

        public UserContext User { get; set; }
        public ICommand LogoutCommand { get; private set; }

        private async void OnLogout()
        {
            var result = await _authenticationService.SignOutAsync();
            if (!result.IsLoggedOn) await this.NavigationService.NavigateAsync("/LoginPage");
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            var user = parameters["User"] as UserContext;
            this.User = user;
        }
    }
}

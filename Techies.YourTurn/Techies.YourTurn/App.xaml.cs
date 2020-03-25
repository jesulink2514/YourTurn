using Prism;
using Prism.Ioc;
using Techies.YourTurn.Security;
using Techies.YourTurn.ViewModels;
using Techies.YourTurn.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Techies.YourTurn
{
    public partial class App
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync("LoginPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();

            containerRegistry.Register<IAuthenticationService, AuthenticationService>();

            containerRegistry.RegisterInstance(new AzureADB2COptions
            {
                AuthorityBase = "https://stechies.b2clogin.com/tfp/stechies.onmicrosoft.com/",
                B2CHostName = "stechies.onmicrosoft.com",
                ClientId = "378ad4d0-d108-4b5b-b76d-69e4c3db1e22",
                iOSKeyChainGroup = "stechies.group",
                PolicyResetPassword = "B2C_1_PasswordReset",
                PolicySignUpSignIn = "B2C_1_SignUpSignIn",
                Scopes = new[] { "https://stechies.onmicrosoft.com/your.turn/api" },
                Tenant = "stechies.onmicrosoft.com"
            });
        }
    }
}

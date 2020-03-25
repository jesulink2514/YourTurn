using Android.App;
using Android.Content;
using Microsoft.Identity.Client;

namespace Techies.YourTurn.Droid
{
    [Activity]
    [IntentFilter(new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
        DataHost = "auth",
        DataScheme = "msal378ad4d0-d108-4b5b-b76d-69e4c3db1e22")]
    public class MsalActivity : BrowserTabActivity
    {
    }
}
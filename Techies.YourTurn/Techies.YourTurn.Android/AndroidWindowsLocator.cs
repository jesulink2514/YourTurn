using Plugin.CurrentActivity;
using Techies.YourTurn.Security;

namespace Techies.YourTurn.Droid
{
    public class AndroidWindowsLocator: IParentWindowLocator
    {
        public object GetCurrentWindow()
        {
            return CrossCurrentActivity.Current;
        }
    }
}
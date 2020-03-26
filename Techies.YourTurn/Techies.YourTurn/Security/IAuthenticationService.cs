using System.Threading.Tasks;

namespace Techies.YourTurn.Security
{
    public interface IAuthenticationService
    {
        Task<UserContext> SignInAsync();
        Task<UserContext> SignOutAsync();
    }
}
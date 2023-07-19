using Wheel.Models;
using Wheel.Models.Enums;

namespace Wheel.Services
{
    public interface IWheelGameService
    {
        Task RegisterUser(UserRegisterModel user);
        Task<string> LoginUser(UserLoginModel user);
        Task ActivateGame(int id);
        Task Deposit(int id, double amount);

        Task<Prize> SpinTheWheel(int id);
    }
}

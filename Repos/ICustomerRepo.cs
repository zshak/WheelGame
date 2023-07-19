using Wheel.Models;
using Wheel.Models.Enums;

namespace Wheel.Repos
{
    public interface ICustomerRepo
    {
        Task RegisterUser(UserRegisterModel user);
        Task<int> GetUserByLogin(UserLoginModel user);
        Task<int> ActivateGame(int id);

        Task Deposit(int id,  double amount);
        Task<Prize?> GetPrize(int id);
    }
}

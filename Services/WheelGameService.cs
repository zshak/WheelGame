using Microsoft.Extensions.Options;
using Npgsql;
using Wheel.Models;
using Wheel.Models.Connections;
using Wheel.Repos;
using Wheel.Extensions;
using Wheel.JWT;
using Wheel.Models.Exceptions;
using Wheel.Models.Enums;

namespace Wheel.Services
{
    public class WheelGameService : IWheelGameService
    {
        private readonly ICustomerRepo _customerRepo;
        private readonly IJWTGenerator _JWTGenerator;

        public WheelGameService(ICustomerRepo customerRepo, IJWTGenerator JWTGenerator)
        {
            _customerRepo = customerRepo;
            _JWTGenerator = JWTGenerator;
        }

        public async Task ActivateGame(int id)
        {
            int numRowsAffected = await _customerRepo.ActivateGame(id);
            if (numRowsAffected == -1) throw new UserNotFoundException("User Not Found", 401);
            if (numRowsAffected == 0) throw new GameAlreadyActivatedException("Game Already Activated", 400);
        }

        public async Task Deposit(int id, double amount)
        {
            await _customerRepo.Deposit(id, amount);
        }

        public async Task<string> LoginUser(UserLoginModel user)
        {
            int id = await _customerRepo.GetUserByLogin(user);
            if (id == -1) throw new UserNotFoundException("User Not Found", 401);

            return _JWTGenerator.GenerateToken(user, id);
        }

        public async Task RegisterUser(UserRegisterModel user)
        {
            user.Password = user.Password.HashString();

            await _customerRepo.RegisterUser(user);
        }

        public async Task<Prize> SpinTheWheel(int id)
        {
            Prize? p = await _customerRepo.GetPrize(id);
            if(p == null) throw new NoSpinsLeftException("No Spins Left", 400);
            return (Prize)p;
        }
    }
}

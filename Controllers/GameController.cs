using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Wheel.Models;
using Wheel.Models.Enums;
using Wheel.Services;

namespace Wheel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IWheelGameService _wheelGameService;
        private readonly IValidator<UserRegisterModel> _userRegisterValidation;
        public GameController(IWheelGameService wheelGameService, IValidator<UserRegisterModel> userRegisterValidation)
        {
            _wheelGameService = wheelGameService;
            _userRegisterValidation = userRegisterValidation;
        }

        private int GetIdFromToken(string Authorization)
        {
            Authorization = Authorization.Substring(7, Authorization.Length - 7);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(Authorization);

            var claim = jwtToken.Claims.FirstOrDefault(c => c.Type == "id").Value;
            int id = Convert.ToInt32(claim);
            return id;
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> Activate([FromHeader] string Authorization)
        {
            int id = GetIdFromToken(Authorization);
            await _wheelGameService.ActivateGame(id);
            return Ok("Game Successfully Activated");
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> Deposit([FromHeader] string Authorization, int amount)
        {
            int id = GetIdFromToken(Authorization);
            await _wheelGameService.Deposit(id, amount);
            return Ok("Bet Succesfully placed");
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> SpinTheWheel([FromHeader] string Authorization)
        {
            int id = GetIdFromToken(Authorization);
            Prize p = await (_wheelGameService.SpinTheWheel(id));
            return Ok("Congratulations: " + p);
        }
    }
}

using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Wheel.Models;
using Wheel.Services;

namespace Wheel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IWheelGameService _wheelGameService;
        private readonly IValidator<UserRegisterModel> _userRegisterValidation;
        public UserController(IWheelGameService wheelGameService, IValidator<UserRegisterModel> userRegisterValidation)
        {
            _wheelGameService = wheelGameService;   
            _userRegisterValidation = userRegisterValidation;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RegisterUser(UserRegisterModel user)
        {
            ValidationResult res = _userRegisterValidation.Validate(user);
            if (!res.IsValid)
            {
                return BadRequest(res.Errors);
            }
            await _wheelGameService.RegisterUser(user);
            return Ok("User Successfully Registered");

        }

        [HttpPost("[action]")]
        public async Task<IActionResult> LoginUSer(UserLoginModel user)
        {
            return Ok(await _wheelGameService.LoginUser(user));
        }

    }
}

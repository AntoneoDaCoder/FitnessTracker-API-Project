using Microsoft.AspNetCore.Mvc;
using FitnessTracker.Application.DTO;
using FitnessTracker.Core.Abstractions;
namespace FitnessTracker.AuthenticationService.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService) => _authService = authService;

        [HttpPost("login")]
        public async Task<IActionResult> LoginUserAsync([FromBody] UserLoginDto loginDto)
        {
            if (!await _authService.ValidateUserAsync(loginDto))
                return Unauthorized();
            return Ok(new { Token =  _authService.GetUserToken() });
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserAsync([FromBody] UserRegisterDto registerDto)
        {
            var result = await _authService.RegisterUserAsync(registerDto);
            if (result.Count > 0)
            {
                foreach (var error in result)
                    ModelState.TryAddModelError(error.Key, error.Value);
                return BadRequest(ModelState);
            }
            return StatusCode(201);
        }
    }
}

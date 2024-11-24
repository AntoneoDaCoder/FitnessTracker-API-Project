using FitnessTracker.Core.Abstractions;
using FitnessTracker.Core.Models;
using FitnessTracker.Application.DTO;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using System.Text;
namespace FitnessTracker.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IValidator<IRegisterData> _userRegisterValidator;
        private readonly IValidator<ILoginData> _userLoginValidator;
        private readonly IConfiguration _configuration;
        private User? _user;
        public AuthService(UserManager<User> userManager, IValidator<IRegisterData> registerValidator,
            IValidator<ILoginData> loginValidator, IConfiguration conf)
        {
            _userManager = userManager;
            _userRegisterValidator = registerValidator;
            _userLoginValidator = loginValidator;
            _configuration = conf;
        }
        private static void MapUserDto(User dst, IRegisterData src)
        {
            dst.UserName = src.Login;
            dst.Age = src.Age;
            dst.Weight = src.Weight;
            dst.Height = src.Height;
            dst.Gender = src.Gender;
        }
        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var tokenOptions = new JwtSecurityToken
            (
            issuer: jwtSettings["validIssuer"],
            audience: jwtSettings["validAudience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expires"])),
            signingCredentials: signingCredentials
            );
            return tokenOptions;
        }
        private List<Claim> GetClaims()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, _user.UserName) };
            return claims;
        }
        private static SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET"));
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }
        public string GetUserToken()
        {
            var signingCredentials = GetSigningCredentials();
            var claims = GetClaims();
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }
        public async Task<bool> ValidateUserAsync(ILoginData user)
        {
            var validationResult = await _userLoginValidator.ValidateAsync(user);
            if (validationResult.IsValid)
            {
                _user = await _userManager.FindByNameAsync(user.Login);
                return (_user is not null && await _userManager.CheckPasswordAsync(_user, user.Password));
            }
            return false;
        }
        public async Task<Dictionary<string, string>> RegisterUserAsync(IRegisterData userToRegister)
        {
            var validationResult = await _userRegisterValidator.ValidateAsync(userToRegister);
            var errors = new Dictionary<string, string>();
            if (validationResult.IsValid)
            {
                var mappedUser = new User();
                MapUserDto(mappedUser, userToRegister);
                var user = await _userManager.CreateAsync(mappedUser, userToRegister.Password);
                if (!user.Succeeded)
                    foreach (var error in user.Errors)
                        if (!errors.TryAdd(error.Code, error.Description))
                            errors[error.Code] += "; " + error.Description;
            }
            else
                foreach (var error in validationResult.Errors)
                    if (!errors.TryAdd(error.PropertyName, error.ErrorMessage))
                        errors[error.PropertyName] += "; " + error.ErrorMessage;
            return errors;
        }
    }
}

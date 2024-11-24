namespace FitnessTracker.Core.Abstractions
{
    public interface IAuthService
    {
        public string GetUserToken();
        public Task<bool> ValidateUserAsync(ILoginData user);
        public Task<Dictionary<string,string>> RegisterUserAsync(IRegisterData user);
    }
}

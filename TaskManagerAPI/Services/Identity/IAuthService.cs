using TaskManagerAPI.Models.Identity;

namespace TaskManagerAPI.Services.Identity
{
    public interface IAuthService
    {
        Task<string> Login(LoginRequest request);
    }
}

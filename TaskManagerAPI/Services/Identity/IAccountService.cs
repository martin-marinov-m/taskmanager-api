using TaskManagerAPI.Models.Identity;

namespace TaskManagerAPI.Services.Identity
{
    public interface IAccountService
    {
        Task Register(RegisterRequest request);
    }
}
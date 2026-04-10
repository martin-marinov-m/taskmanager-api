using Microsoft.AspNetCore.Identity;
using TaskManagerAPI.Constants;
using TaskManagerAPI.Models.Identity;

namespace TaskManagerAPI.Services.Identity
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<TaskManagerUser> _userManager;

        public AccountService(UserManager<TaskManagerUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task Register(RegisterRequest request)
        {
            if (await _userManager.FindByEmailAsync(request.Email) != null)
                throw new InvalidOperationException("User already exists");

            var validRoles = new HashSet<string>() { Roles.Admin, Roles.TeamLeader, Roles.Developer };

            if (!validRoles.Contains(request.Role))
                throw new ArgumentException($"Role {request.Role} is invalid");

            var user = new TaskManagerUser
            {
                Email = request.Email,
                UserName = request.Email,
                EmailConfirmed = true,
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);

            if (!createResult.Succeeded)
                throw new InvalidOperationException($"Failed to create User with email: {user.Email}. Errors: {string.Join(",", createResult.Errors)}");

            var roleAssignResult = await _userManager.AddToRoleAsync(user, request.Role);

            if (!roleAssignResult.Succeeded)
                throw new InvalidOperationException($"Failed to assign role to User with email: {user.Email}. Errors: {string.Join(",", roleAssignResult.Errors)}");
        }
    }
}

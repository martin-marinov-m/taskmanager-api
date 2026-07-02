using Microsoft.AspNetCore.Identity;
using TaskManagerAPI.Constants;
using TaskManagerAPI.GlobalExceptionHandler.Exceptions.Business;
using TaskManagerAPI.GlobalExceptionHandler.Exceptions.Identity;
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
                throw new UserAlreadyExistsException(request.Email);

            var validRoles = new HashSet<string>() { Roles.Admin, Roles.TeamLeader, Roles.Developer };

            if (!validRoles.Contains(request.Role))
                throw new ParameterValidationException("Invalid role", "Role", request.Role);

            var user = new TaskManagerUser
            {
                Email = request.Email,
                UserName = request.Email,
                EmailConfirmed = true,
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);

            if (!createResult.Succeeded)
                throw new UserCreationFailedException(user.Email, createResult.Errors.Select(e => e.Description));

            var roleAssignResult = await _userManager.AddToRoleAsync(user, request.Role);

            if (!roleAssignResult.Succeeded)
                throw new RoleAssignmentFailedException(user.Email, request.Role, roleAssignResult.Errors.Select(e => e.Description));
        }
    }
}

using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TaskManagerAPI.Constants;
using TaskManagerAPI.GlobalExceptionHandler.Exceptions.Identity;
using TaskManagerAPI.GlobalExceptionHandler.Exceptions.Server;
using TaskManagerAPI.Models.Identity;
using TaskManagerAPI.Options;

namespace TaskManagerAPI.Data.Configurations.Identity
{
    public static class UserSeeder
    {
        public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<TaskManagerUser>>();

            var seededEmailsOptions = serviceProvider.GetRequiredService<IOptions<SeededEmailsOptions>>().Value ?? throw new InvalidConfigurationException("SeededEmails");

            if (string.IsNullOrWhiteSpace(seededEmailsOptions.Admin))
                throw new InvalidConfigurationException("SeededEmails:Admin");

            if (string.IsNullOrWhiteSpace(seededEmailsOptions.TeamLeader))
                throw new InvalidConfigurationException("SeededEmails:TeamLeader");

            if (string.IsNullOrWhiteSpace(seededEmailsOptions.Developer))
                throw new InvalidConfigurationException("SeededEmails:Developer");

            var seededPasswordsOptions = serviceProvider.GetRequiredService<IOptions<SeededPasswordsOptions>>().Value ?? throw new InvalidConfigurationException("SeededPasswords");

            if (string.IsNullOrWhiteSpace(seededPasswordsOptions.Admin))
                throw new InvalidConfigurationException("SeededPasswords:Admin");

            if (string.IsNullOrWhiteSpace(seededPasswordsOptions.TeamLeader))
                throw new InvalidConfigurationException("SeededPasswords:TeamLeader");

            if (string.IsNullOrWhiteSpace(seededPasswordsOptions.Developer))
                throw new InvalidConfigurationException("SeededPasswords:Developer");

            //Admin role
            await CreateUserWithRole(userManager, seededEmailsOptions.Admin, seededPasswordsOptions.Admin, Roles.Admin);

            //TeamLeader role
            await CreateUserWithRole(userManager, seededEmailsOptions.TeamLeader, seededPasswordsOptions.TeamLeader, Roles.TeamLeader);

            //Developer role
            await CreateUserWithRole(userManager, seededEmailsOptions.Developer, seededPasswordsOptions.Developer, Roles.Developer);
        }

        private static async Task CreateUserWithRole(UserManager<TaskManagerUser> userManager, string email, string password, string role)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new TaskManagerUser()
                {
                    Email = email,
                    UserName = email,
                    EmailConfirmed = true,
                };

                var createResult = await userManager.CreateAsync(user, password);

                if (!createResult.Succeeded)
                    throw new UserCreationFailedException(user.Email, createResult.Errors.Select(e => e.Description));

                var roleAssignResult = await userManager.AddToRoleAsync(user, role);

                if (!roleAssignResult.Succeeded)
                    throw new RoleAssignmentFailedException(user.Email, role, roleAssignResult.Errors.Select(e => e.Description));
            }
        }
    }
}
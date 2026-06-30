using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TaskManagerAPI.Constants;
using TaskManagerAPI.Models.Identity;
using TaskManagerAPI.Options;

namespace TaskManagerAPI.Data.Configurations.Identity
{
    public static class UserSeeder
    {
        public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<TaskManagerUser>>();

            var seededEmailsOptions = serviceProvider.GetRequiredService<IOptions<SeededEmailsOptions>>().Value ?? throw new KeyNotFoundException("SeededEmails configuration was not found.");

            if (string.IsNullOrWhiteSpace(seededEmailsOptions.Admin))
                throw new KeyNotFoundException("Email for Admin was not found.");

            if (string.IsNullOrWhiteSpace(seededEmailsOptions.TeamLeader))
                throw new KeyNotFoundException("Email for TeamLeader was not found.");

            if (string.IsNullOrWhiteSpace(seededEmailsOptions.Developer))
                throw new KeyNotFoundException("Email for Developer was not found.");

            var seededPasswordsOptions = serviceProvider.GetRequiredService<IOptions<SeededPasswordsOptions>>().Value ?? throw new KeyNotFoundException("SeededPasswords configuration was not found."); 

            if (string.IsNullOrWhiteSpace(seededPasswordsOptions.Admin))
                throw new KeyNotFoundException("Password for Admin was not found.");

            if (string.IsNullOrWhiteSpace(seededPasswordsOptions.TeamLeader))
                throw new KeyNotFoundException("Password for TeamLeader was not found.");

            if (string.IsNullOrWhiteSpace(seededPasswordsOptions.Developer))
                throw new KeyNotFoundException("Password for Developer was not found.");

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
                    throw new Exception($"Failed to create User with email: {email}. Errors: {string.Join(",", createResult.Errors)}");

                var roleAssignResult = await userManager.AddToRoleAsync(user, role);

                if (!roleAssignResult.Succeeded)
                    throw new Exception($"Failed to assigned role \'{role}\' to user with email: {email}. Errors: {string.Join(",", roleAssignResult.Errors)}");
            }
        }
    }
}

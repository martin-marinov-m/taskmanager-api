using Microsoft.AspNetCore.Identity;
using TaskManagerAPI.Constants;
using TaskManagerAPI.Models.Identity;

namespace TaskManagerAPI.Data.Identity
{
    public static class UserSeeder
    {
        public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<TaskManagerUser>>();

            var config = serviceProvider.GetRequiredService<IConfiguration>();

            //Admin role
            var adminEmail = config["SeededEmails:Admin"] ?? throw new KeyNotFoundException("Email for Admin was not Found.");
            var adminPassword = config["SeededPasswords:Admin"] ?? throw new KeyNotFoundException("Password for Admin was not Found.");
            await CreateUserWithRole(userManager, adminEmail, adminPassword, Roles.Admin);

            //TeamLeader role
            var teamLeaderEmail = config["SeededEmails:TeamLeader"] ?? throw new KeyNotFoundException("Email for TeamLeader was not Found.");
            var teamLeaderPassword = config["SeededPasswords:TeamLeader"] ?? throw new KeyNotFoundException("Password for TeamLeader was not Found.");

            await CreateUserWithRole(userManager, teamLeaderEmail, teamLeaderPassword, Roles.TeamLeader);

            //Developer role
            var developerEmail = config["SeededEmails:Developer"] ?? throw new KeyNotFoundException("Email for Developer was not Found.");
            var developerPassword = config["SeededPasswords:Developer"] ?? throw new KeyNotFoundException("Password for Developer was not Found.");
            await CreateUserWithRole(userManager, developerEmail, developerPassword, Roles.Developer);


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

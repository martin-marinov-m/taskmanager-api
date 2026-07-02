using Microsoft.AspNetCore.Identity;
using TaskManagerAPI.Constants;
using TaskManagerAPI.GlobalExceptionHandler.Exceptions.Identity;

namespace TaskManagerAPI.Data.Configurations.Identity
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await CreateRoleAsync(roleManager, Roles.Admin);
            await CreateRoleAsync(roleManager, Roles.TeamLeader);
            await CreateRoleAsync(roleManager, Roles.Developer);
        }

        private static async Task CreateRoleAsync(RoleManager<IdentityRole> roleManager, string role)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(role));

                if (!result.Succeeded)
                    throw new RoleCreationFailedException(role, result.Errors.Select(e => e.Description));
            }
        }
    }
}
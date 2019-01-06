using Microsoft.AspNetCore.Identity;

namespace MvcWithAuthorisation.Data
{
    public static class DataSeeder
    {
        public static void Seed(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        public static void SeedUsers(UserManager<IdentityUser> userManager)
        {
            if (userManager.FindByNameAsync("user@graphity.com").Result != null)
            {
                return;
            }

            var user = new IdentityUser
            {
                UserName = "user@graphity.com",
                Email = "user@graphity.com"
            };

            userManager.CreateAsync(user, "Password123!").Wait();

            user = new IdentityUser
            {
                UserName = "admin@graphity.com",
                Email = "admin@dontcare.com"
            };

            var result = userManager.CreateAsync(user, "Password123!").Result;

            if (result.Succeeded)
            {
                userManager.AddToRoleAsync(user, "admin").Wait();
            }
        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (roleManager.RoleExistsAsync("admin").Result)
            {
                return;
            }

            var adminRole = new IdentityRole
            {
                Name = "admin"
            };
            roleManager.CreateAsync(adminRole).Wait();

            var basicRole = new IdentityRole
            {
                Name = "basic"
            };
            roleManager.CreateAsync(basicRole).Wait();
        }
    }
}
using Microsoft.AspNetCore.Identity;
using SmileScript.Models; // If you have a specific ApplicationUser model, use that namespace.

namespace SmileScript.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // --- Create Roles if they don't exist ---
            string[] roleNames = { "Admin", "Author", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // --- Create a default Admin User ---
            var adminUser = await userManager.FindByEmailAsync("admin@smilescript.com");
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = "admin@smilescript.com",
                    Email = "admin@smilescript.com",
                    EmailConfirmed = true
                };
                // IMPORTANT: Change this password!
                await userManager.CreateAsync(adminUser, "AdminPassword123!");

                // Assign the 'Admin' role
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // --- Create a default Author User ---
            var authorUser = await userManager.FindByEmailAsync("author@smilescript.com");
            if (authorUser == null)
            {
                authorUser = new IdentityUser
                {
                    UserName = "author@smilescript.com",
                    Email = "author@smilescript.com",
                    EmailConfirmed = true
                };
                // IMPORTANT: Change this password!
                await userManager.CreateAsync(authorUser, "AuthorPassword123!");

                // Assign the 'Author' role
                await userManager.AddToRoleAsync(authorUser, "Author");
            }
        }
    }
}
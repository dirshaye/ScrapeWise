using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Seeds default users and roles for the application using ASP.NET Core Identity.
/// </summary>
public static class SeedData
{
    /// <summary>
    /// Fix existing users to use email as username for consistency with login system
    /// </summary>
    private static async Task FixExistingUsersAsync(UserManager<MyUser> userManager)
    {
        Console.WriteLine("🔧 Checking existing users for email/username consistency...");
        
        // Get all users
        var allUsers = userManager.Users.ToList();
        
        foreach (var user in allUsers)
        {
            if (user.UserName != user.Email && !string.IsNullOrEmpty(user.Email))
            {
                Console.WriteLine($"🔄 Updating user: {user.Email} (was username: {user.UserName})");
                
                var oldUserName = user.UserName;
                user.UserName = user.Email;
                user.NormalizedUserName = user.Email.ToUpper();
                
                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    Console.WriteLine($"✅ Successfully updated {user.Email}");
                }
                else
                {
                    Console.WriteLine($"❌ Failed to update {user.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else if (user.UserName == user.Email)
            {
                Console.WriteLine($"ℹ️ User {user.Email} already has consistent username/email");
            }
        }
    }
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<MyUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // STEP 1: Fix existing users to use email as username
        await FixExistingUsersAsync(userManager);

        // STEP 2: Seed roles
        string[] roles = new[] { "Admin", "User", "Guest" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Seed admin user
        var adminEmail = "admin@scrapewise.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new MyUser
            {
                UserName = adminEmail, // Use email as username for consistency
                Email = adminEmail,
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = await userManager.CreateAsync(admin, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
                Console.WriteLine($"✅ Admin user created successfully: {adminEmail} / Admin123!");
            }
            else
            {
                Console.WriteLine($"❌ Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
        else
        {
            // Update existing user to use email as username
            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin.UserName != adminEmail)
            {
                existingAdmin.UserName = adminEmail;
                existingAdmin.NormalizedUserName = adminEmail.ToUpper();
                await userManager.UpdateAsync(existingAdmin);
                Console.WriteLine($"🔄 Updated admin user to use email as username: {adminEmail}");
            }
            Console.WriteLine($"ℹ️ Admin user already exists: {adminEmail}");
        }

        // Seed regular user
        var userEmail = "user@scrapewise.com";
        if (await userManager.FindByEmailAsync(userEmail) == null)
        {
            var user = new MyUser
            {
                UserName = userEmail, // Use email as username for consistency
                Email = userEmail,
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = await userManager.CreateAsync(user, "User123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "User");
                Console.WriteLine($"✅ Regular user created successfully: {userEmail} / User123!");
            }
            else
            {
                Console.WriteLine($"❌ Failed to create regular user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
        else
        {
            // Update existing user to use email as username
            var existingUser = await userManager.FindByEmailAsync(userEmail);
            if (existingUser.UserName != userEmail)
            {
                existingUser.UserName = userEmail;
                existingUser.NormalizedUserName = userEmail.ToUpper();
                await userManager.UpdateAsync(existingUser);
                Console.WriteLine($"🔄 Updated regular user to use email as username: {userEmail}");
            }
            Console.WriteLine($"ℹ️ Regular user already exists: {userEmail}");
        }
    }
} 
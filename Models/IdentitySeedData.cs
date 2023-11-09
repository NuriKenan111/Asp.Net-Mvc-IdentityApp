using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Models;

public class IdentitySeedData
{
    public const string adminUser = "Admin";
    public const string adminPassword = "admin123";
    public static async void IdentityTestUser(IApplicationBuilder app){
        var context = app.ApplicationServices
        .CreateScope().ServiceProvider.GetRequiredService<IdentityContext>();
        
        if(context.Database.GetAppliedMigrations().Any()){
            context.Database.Migrate();
        }

        var userManager = app.ApplicationServices
        .CreateScope().ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        var user = await userManager.FindByNameAsync(adminUser);

        if(user == null){
            user = new AppUser{
                FullName = "Kenan Nuri",
                UserName = adminUser,
                Email = "kenan@gmail.com",
                PhoneNumber = "055648",
            };
            
            await userManager.CreateAsync(user,adminPassword);                
        }
    
    }

}

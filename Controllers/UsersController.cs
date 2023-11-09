using IdentityApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IdentityApp.Controllers;

public class UsersController : Controller
{
    private UserManager<IdentityUser> _userManager;
    public UsersController(UserManager<IdentityUser> usermanager)
    {
        _userManager = usermanager;
    }
    public IActionResult Index()
    {
        return View(_userManager.Users);
    }

    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(CreateViewModel model)
    {
        if(ModelState.IsValid){
            var user = new IdentityUser{
                UserName = model.UserName,
                Email = model.Email,
            };
            IdentityResult result = await _userManager.CreateAsync(user,model.Password);
            if(result.Succeeded){
                return RedirectToAction("Index");
            }
            else{
                foreach(IdentityError error in result.Errors){
                    ModelState.AddModelError("",error.Description);
                }
            }
        
        }
        return View(model);
    }

}
using IdentityApp.Models;
using IdentityApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IdentityApp.Controllers;

public class UsersController : Controller
{
    private UserManager<AppUser> _userManager;
    public UsersController(UserManager<AppUser> usermanager)
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
            var user = new AppUser{
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName
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
    public async Task<IActionResult> Edit(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(new EditViewModel
        {
            Id = id,
            FullName = user.FullName,
            Email = user.Email
            
        });
    }
    [HttpPost]
    public async Task<IActionResult> Edit(string id,EditViewModel user){

        if(id != user.Id)
            return NotFound();
        
        if(ModelState.IsValid){
            var _user = await _userManager.FindByIdAsync(id);
            if(_user != null){

                _user.FullName = user.FullName;
                _user.Email = user.Email;

                var result = await _userManager.UpdateAsync(_user);

                if(result.Succeeded && !string.IsNullOrEmpty(user.Password)){
                    await _userManager.RemovePasswordAsync(_user);
                    await _userManager.AddPasswordAsync(_user,user.Password);
                }

                if(result.Succeeded){
                    return RedirectToAction("Index");
                }
                else{
                    foreach(IdentityError error in result.Errors){
                        ModelState.AddModelError("",error.Description);
                    }
                }
            }
            
        }
        return View(user);
    }

    public async Task<IActionResult> Delete(string id){
        var user = await _userManager.FindByIdAsync(id);

        if(user != null){
            var result = await _userManager.DeleteAsync(user);
        }

        return RedirectToAction("Index");
       
    
    }
}
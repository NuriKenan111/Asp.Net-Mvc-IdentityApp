using IdentityApp.Models;
using IdentityApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IdentityApp.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private UserManager<AppUser> _userManager;
    private RoleManager<AppRole> _roleManager;
    public UsersController(UserManager<AppUser> usermanager,RoleManager<AppRole> roleManager)
    {
        _userManager = usermanager;
        _roleManager = roleManager;
    }
    public IActionResult Index()
    {
        return View(_userManager.Users);
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
        ViewBag.Roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        return View(new EditViewModel
        {
            Id = id,
            FullName = user.FullName,
            Email = user.Email,
            SelectedRole = await _userManager.GetRolesAsync(user)
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
                    await _userManager.RemoveFromRolesAsync(_user,await _userManager.GetRolesAsync(_user));
                    if(user.SelectedRole != null){
                        await _userManager.AddToRolesAsync(_user,user.SelectedRole);
                    }
                    
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
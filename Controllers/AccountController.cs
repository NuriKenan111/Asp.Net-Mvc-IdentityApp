using IdentityApp.Models;
using IdentityApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers;


public class AccountController : Controller
{
    private UserManager<AppUser> _userManager;
    private RoleManager<AppRole> _roleManager;
    private SignInManager<AppUser> _signInManager;
    private IEmailSender _emailSender;
    public AccountController(UserManager<AppUser> usermanager,
    RoleManager<AppRole> roleManager,
    SignInManager<AppUser> signInManager
    , IEmailSender emailSender)
    {
        _userManager = usermanager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {   
        if (ModelState.IsValid){
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null){
                await _signInManager.SignOutAsync();

                if(!await _userManager.IsEmailConfirmedAsync(user)){
                    ModelState.AddModelError("", "Email not confirmed yet");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);  

                if (result.Succeeded){
                    await _userManager.ResetAccessFailedCountAsync(user);   
                    await _userManager.SetLockoutEndDateAsync(user, null);   

                    return RedirectToAction("Index", "Home");
                }    
                else if(result.IsLockedOut){
                    var lockoutDate = await _userManager.GetLockoutEndDateAsync(user);
                    var timeLeft = lockoutDate.Value - DateTime.UtcNow;
                    ModelState.AddModelError("", $"You have been locked out. Please try again in {timeLeft.Minutes} minutes");
                }
                else{
                    ModelState.AddModelError("", "User Name or Password is not correct");

                }
            }
            else{
                ModelState.AddModelError("", "User Name or Password is not correct");
            }
        }
        return View(model);
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
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName
            };
            IdentityResult result = await _userManager.CreateAsync(user,model.Password);
            if(result.Succeeded){
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action("ConfirmEmail","Account",new {userId = user.Id,token = token},Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email, "Hesap Onayı", 
                    $"Lütfen email hesabınızı onaylamak için linke <a href='http://localhost:5243{url}'>tıklayınız.</a>");

                TempData["message"] = "Check your email and confirm your account";
                return RedirectToAction("Login","Account");
            }
            else{
                foreach(IdentityError error in result.Errors){
                    ModelState.AddModelError("",error.Description);
                }
            }
        
        }
        return View(model);
    }

    public async Task<IActionResult> ConfirmEmail(string Id,string token){
        if(Id == null || token == null){
            TempData["message"] = "User Id and token are required";   
            return View();
        }
        var user = await _userManager.FindByIdAsync(Id);

        if(user != null){

            var result = await _userManager.ConfirmEmailAsync(user,token);

            if(result.Succeeded){
                TempData["message"] = "Your email has been confirmed";
                return View();
            }
        }
        TempData["message"] = "Your account not found";
        return View();

    }
    public async Task<IActionResult> Logout(){
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrgAPI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrgAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        UserManager<IdentityUser> userManager;
        SignInManager<IdentityUser> signInManager;

        public AccountController(UserManager<IdentityUser> _userManager, SignInManager<IdentityUser> _signInManager)
        {
            signInManager = _signInManager;
            userManager = _userManager;  
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new IdentityUser()
                    {
                        UserName = model.UserName,
                        Email = model.Email
                    };
                    var userResult = await userManager.CreateAsync(user, model.Password);
                    if (userResult.Succeeded)
                    {
                        var roleResult = await userManager.AddToRoleAsync(user, "User");
                        if (roleResult.Succeeded)
                        {
                            return Ok(user);
                        }
                    } 
                    else
                    {
                        foreach(var err in userResult.Errors)
                        {
                            ModelState.AddModelError("", err.Description);
                        };
                        return BadRequest(ModelState.Values);
                    }
                }
                return BadRequest(ModelState.Values);
            }
            catch (Exception E)
            {
                ModelState.AddModelError("", E.Message);
                return BadRequest(ModelState.Values);
            }
        }

        [HttpPost("signIn")]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            if (ModelState.IsValid)
            {
                var signInResult = await signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
                if (signInResult.Succeeded)
                    return Ok();
            }
            return BadRequest(ModelState);
        }

        [HttpPost("signOut")]
        public async Task<IActionResult> SignOut()
        {
            await signInManager.SignOutAsync();
            return NoContent();
        }
    }
}

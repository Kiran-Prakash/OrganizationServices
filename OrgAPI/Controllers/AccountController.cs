using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OrgAPI.ViewModel;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
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
                {
                    var user = await userManager.FindByNameAsync(model.UserName);
                    var roles = await userManager.GetRolesAsync(user);
                    IdentityOptions identityOptions = new IdentityOptions();
                    var claims = new Claim[]
                    {
                        new Claim("Lid","123456789"),
                        new Claim(identityOptions.ClaimsIdentity.UserIdClaimType,user.Id),
                        new Claim(identityOptions.ClaimsIdentity.UserNameClaimType,user.UserName),
                        new Claim(identityOptions.ClaimsIdentity.RoleClaimType,roles[0])
                    };
                    var signingkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this-is-my-secret-key"));
                    var signingCredentials = new SigningCredentials(signingkey, SecurityAlgorithms.HmacSha256);
                    var jwt = new JwtSecurityToken(signingCredentials: signingCredentials, claims: claims, expires: DateTime.Now.AddMinutes(30));
                    var obj = new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(jwt),
                        UserId = user.Id,
                        UserName = user.UserName,
                        Role = roles[0]
                    };
                    return Ok(obj);
                }
                    
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

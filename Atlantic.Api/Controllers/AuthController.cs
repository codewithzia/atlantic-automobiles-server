using Atlantic.Data.Models.Auth;
using Atlantic.Data.Models.Dtos;
using Atlantic.Data.Services;
using DMS.Data.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Atlantic.Api.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLogin request)
        {
            if (ModelState.IsValid)
            {
                return Ok(await _authService.Login(request));
            }

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("LoginByOtp")]
        public async Task<IActionResult> LoginByOtp(UserLogin request)
        {
            if (string.IsNullOrEmpty(request.Otp))
            {
                return BadRequest("OTP is required");
            }
            if (ModelState.IsValid)
            {
                return Ok(await _authService.LoginByOtp(request));
            }

            return Ok();
        }

        [HttpPost("roles")]
        public async Task<IActionResult> Roles()
        {
            var roles = await _authService.Roles();
            return Ok(roles);
        }

        [HttpGet]
        [Route("~/Account/Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }


        [HttpDelete("user/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            return Ok(await _authService.RemoveUser(id));
        }

        [HttpPost("UpdatePin")]
        public async Task<IActionResult> UpdatePin(UpdatePinDto updatePin)
        {
            var response = await _authService.UpdatePin(updatePin);
            return Ok(response);
        }
        [HttpPost("CheckPin")]
        public async Task<IActionResult> CheckPin(UpdatePinDto updatePin)
        {
            return Ok(await _authService.CheckPin(updatePin));
        }
        [HttpPost("HavePin")]
        public async Task<IActionResult> HavePin()
        {
            return Ok(await _authService.HavePin());
        }
    }

}

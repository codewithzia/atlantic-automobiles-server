using Atlantic.Data.Models.Auth;
using Atlantic.Data.Models.Dtos;
using Atlantic.Data.Models.Requests;
using Atlantic.Data.Models.Responses;
using Atlantic.Data.Models.System;
using Atlantic.Data.Repositories;
using DMS.Data.Models.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Atlantic.Data.Services.Auth
{
    public interface IAuthService
    {
        public Task<LoginResponse> Login(UserLogin request);
        public Task<LoginResponse> LoginByOtp(UserLogin request);
        public Task<UserDto> CreateAdmin(SuperAdminRequest superAdminRequest);
        public Task<List<string>> Roles();
        public Task<GenericResponse> RemoveUser(string userId);
        public Task<GenericResponse> UpdatePin(UpdatePinDto updatePin);
        public Task<bool> CheckPin(UpdatePinDto updatePin);
        public Task<bool> HavePin();
        public Task<LoginResponse> ChangePassword(ChangePassword changePassword);
    }
    public class AuthService : IAuthService
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private IUnitOfWork _unitOfWork;
        private IOtpService _otpService;
        private RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessorService _httpAccessor;
        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IHttpContextAccessorService httpAccessor,
            IUnitOfWork unitOfWork, IOtpService otpService, RoleManager<ApplicationRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _otpService = otpService;
            _roleManager = roleManager;
            _configuration = configuration;
            _httpAccessor = httpAccessor;
        }

        public async Task<UserDto> CreateAdmin(SuperAdminRequest superAdminRequest)
        {
            try
            {
                var user = new ApplicationUser
                {
                    UserName = "SuperAdmin",
                    Email = superAdminRequest.Email,
                    PhoneNumber = superAdminRequest.Phone,
                    PasswordValidity = DateTime.Now.AddDays(90),
                    Status = true,
                    IsSystemAdmin = true,
                };

                IdentityResult result = await _userManager.CreateAsync(user, superAdminRequest.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
                return new UserDto
                {
                    Id = user.Id,
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,
                    Email = user.Email,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<LoginResponse> Login(UserLogin request)
        {
            try
            {
                ApplicationUser appUser = await _userManager.FindByEmailAsync(request.Email);
                if (appUser != null && appUser.Status == true)
                {
                    SignInResult result = await _signInManager.PasswordSignInAsync(appUser, request.Password, false, false);
                    if (result.Succeeded)
                    {
                        string token = await CreateToken(appUser);

                        //Response.Cookies.Append(key: "token", value: token, new CookieOptions
                        //{
                        //    HttpOnly = true
                        //});
                        // send otp 
                        var optRequest = new SendOtpRequest()
                        {
                            Email = request.Email,
                            PhoneNumber = null
                        };
                        await _otpService.SendOtp(optRequest);
                        return new LoginResponse
                        {
                            Email = request.Email,
                            Message = "Otp Sent",
                            Token = token
                        };
                    }
                    else
                    {

                        throw new Exception("Login Failed: Invalid Email or Password");
                    }

                }
                throw new Exception("Login Failed: Invalid Email");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<LoginResponse> LoginByOtp(UserLogin request)
        {
            try
            {
                ApplicationUser appUser = await _userManager.FindByEmailAsync(request.Email);
                if (appUser != null && appUser.Status == true)
                {
                    SignInResult result = await _signInManager.PasswordSignInAsync(appUser, request.Password, false, false);
                    if (result.Succeeded)
                    {
                        var getOtp = await _otpService.GetByEmail(request.Email);
                        if (getOtp.Token.ToLower() != request.Otp.ToLower())
                        {
                            throw new Exception("Verification Failed: Otp not matched");
                        }
                        else if (!_otpService.IsValid(getOtp)) // check user available by this 
                        {
                            throw new Exception("Verification Failed: Otp not valid");
                        }
                        string token = await CreateToken(appUser);

                        //Response.Cookies.Append(key: "token", value: token, new CookieOptions
                        //{
                        //    HttpOnly = true
                        //});

                        return new LoginResponse
                        {
                            Token = token,
                            Email = request.Email,
                            UserDto = new UserDto
                            {
                                Id = appUser.Id,
                                Firstname = appUser.Firstname,
                                Lastname = appUser.Lastname,
                                Email = appUser.Email,
                                UserName = appUser.UserName,
                                PhoneNumber = appUser.PhoneNumber,
                            }
                        };
                    }
                    else
                    {
                        throw new Exception("Login Failed: Invalid Email or Password");
                    }

                }
                throw new Exception("Login Failed: Invalid Email");
            }
            catch (Exception)
            {

                throw;
            }

        }

        private async Task<string> CreateToken(ApplicationUser appUser)
        {
            IdentityOptions _options = new IdentityOptions();
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, appUser.Email),
                new Claim(_options.ClaimsIdentity.UserIdClaimType, appUser.Id.ToString()),
                new Claim(_options.ClaimsIdentity.UserNameClaimType, appUser.UserName),
                new Claim("UserId", appUser?.Id.ToString()??string.Empty),
                new Claim("Username", appUser?.UserName.ToString()??string.Empty),
                new Claim("IsSystemAdmin",appUser?.IsSystemAdmin.ToString()??"false")

            };
            string roleAgent = "";

            var userRoles = await _userManager.GetRolesAsync(appUser);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (Claim roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                    claims.Add(new Claim("Role", role.Name));
                    roleAgent = role.Name;
                }
            }
            // add claims
            var agent = _unitOfWork.UserProfileRepository.Search().Where(x => x.UserId == appUser.Id.ToString()).FirstOrDefault();
            if (agent != null)
            {
                claims.Add(new Claim("AgentId", agent.ID));
                if (!string.IsNullOrEmpty(agent.ParentAgentId))
                {
                    claims.Add(new Claim("Parentagentid", agent.ParentAgentId));
                }
                claims.Add(new Claim("UserType", agent?.UserType.ToString()));
            }
            claims.Add(new Claim("Country", "MY"));
            claims.Add(new Claim("Currency", "MYR"));

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public async Task<List<string>> Roles()
        {
            try
            {
                return _roleManager.Roles.Select(x => x.Name).ToList();
            }
            catch (Exception)
            {

                throw;
            }
            throw new NotImplementedException();
        }

        public async Task<GenericResponse> RemoveUser(string userId)
        {
            try
            {
                ApplicationUser appUser = await _userManager.FindByIdAsync(userId);
                if (appUser != null)
                {
                    await _userManager.DeleteAsync(appUser);
                }
                else
                {
                    throw new Exception("User not found!");
                }
                return new GenericResponse
                {
                    Message = "User removed successfully!",
                    Success = true
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<GenericResponse> UpdatePin(UpdatePinDto updatePin)
        {
            try
            {
                ApplicationUser appUser = await _httpAccessor.CurrentUser();

                if (appUser.Pin.ToLower() == updatePin.OldPin.ToLower())
                {
                    appUser.Pin = updatePin.NewPin;
                    _userManager.UpdateAsync(appUser);
                    return new GenericResponse
                    {
                        Message = "Pin Updated",
                        Success = true
                    };
                }
                else
                {
                    throw new Exception("Wrong Pin");
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> CheckPin(UpdatePinDto updatePin)
        {
            try
            {
                ApplicationUser appUser = await _httpAccessor.CurrentUser();
                if (string.IsNullOrEmpty(appUser.Pin) || appUser.Pin.ToLower() == updatePin.OldPin.ToLower())
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<bool> HavePin()
        {
            try
            {
                ApplicationUser appUser = await _httpAccessor.CurrentUser();
                if (string.IsNullOrEmpty(appUser.Pin))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<LoginResponse> ChangePassword(ChangePassword changePassword)
        {
            try
            {
                var appUser = await _userManager.FindByIdAsync(_unitOfWork.HttpAccessor().CurrentUserId());
                // check if old password is correct 
                var OldPAssisCorrect = await _userManager.CheckPasswordAsync(appUser, changePassword.OldPassword);
                if (!OldPAssisCorrect)
                {
                    throw new Exception("Password didnot matched");
                }
                var token = await _userManager.GeneratePasswordResetTokenAsync(appUser);
                // change password
                await _userManager.ResetPasswordAsync(appUser, token, changePassword.NewPassword);

                return new LoginResponse
                {
                    Email = appUser.Email,
                    UserDto = new UserDto
                    {
                        Id = appUser.Id,
                        Firstname = appUser.Firstname,
                        Lastname = appUser.Lastname,
                        Email = appUser.Email,
                        UserName = appUser.UserName,
                        PhoneNumber = appUser.PhoneNumber,
                    }
                };
            }
            catch (Exception)
            {

                throw;
            }

        }

    }
}

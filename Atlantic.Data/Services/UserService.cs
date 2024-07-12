using Atlantic.Data.Models;
using Atlantic.Data.Models.Auth;
using Atlantic.Data.Models.Settings;
using Atlantic.Data.Models.System;
using Atlantic.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Entities;

namespace Atlantic.Data.Services
{
    public interface IUserService
    {
        public Task<UserProfile> CreateUser(UserProfile agent);
    }
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private UserManager<ApplicationUser> _userManager;
        private readonly IMailService _mailService;
        private string _siteName;
        private readonly IHostEnvironment _hostEnvironment;
        public UserService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager,
            IMailService mailService, IOptions<DbSettings> options, IHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mailService = mailService;
            _siteName = options.Value.SiteLink;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<UserProfile> CreateUser(UserProfile agent)
        {
            try
            {
                var isAgentAvailabel = _unitOfWork.UserProfileRepository.Search().Where(x => x.Email.ToLower() == agent.Email.ToLower()).FirstOrDefault();
                if (isAgentAvailabel != null)
                {
                    throw new Exception("Agent with this email already exists");
                }
                agent.Serial = await agent.NextSequentialNumberAsync();
                // agent.Code = "agt-" + agent.Serial;
                var _agent = await _unitOfWork.UserProfileRepository.Insert(agent);

                var user = new ApplicationUser
                {
                    UserName = agent.Email,
                    Email = agent.Email,
                    PhoneNumber = agent.Phone,
                    PasswordValidity = DateTime.Now.AddDays(90),
                    CreatedById = _unitOfWork.HttpAccessor().CurrentUserId(),
                    Status = true
                };

                string role = "Agent";
                user.UserProfileId = _agent.ID;
                string password = RandomPassword();
                IdentityResult result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    _agent.UserId = user.Id.ToString();
                    await _userManager.AddToRoleAsync(user, role);
                    string mailBody = $"Dear {_agent.Name}," +
                        $"<br>  Welcome to you DSM system" +
                        $"<br>" +
                        $"<br>Click <a href='{_siteName}'>here</a>  or copy paste this link '{_siteName}' to login." +
                        $"<br>" +
                    $"<br>Your login credentails as below" +
                        $"<br>  <b>Username</b>: {user.UserName}" +
                        $"<br>  <b>Password</b>: {password}" +
                        $"<br>" +
                        $"<br>Thank you";

                    await _mailService.SendEmailAsync(new MailRequest
                    {
                        ToEmail = _agent.Email,
                        Name = _agent.Name,
                        Subject = "Welcome to B2B Portal",
                        Body = mailBody
                    });
                    _agent.UserType = role;
                    await _unitOfWork.UserProfileRepository.Update(_agent);
                    return _agent;
                }
                else
                {
                    throw new Exception("Agent with this email already exists");
                }

            }
            catch (MongoWriteException ex)
            {
                throw new Exception("Agent with this email already exists");
            }

        }
        // create a 6 digit random password
        public string RandomPassword(int length = 6)
        {
            string _allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789@";
            Random randNum = new Random();
            char[] chars = new char[length];
            int allowedCharCount = _allowedChars.Length;
            for (int i = 0; i < length; i++)
            {
                chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
            }
            if (_hostEnvironment.IsDevelopment())
            {
                return new string("123456");
            }
            else
            {
                return new string(chars);
            }      
        }
    }
}

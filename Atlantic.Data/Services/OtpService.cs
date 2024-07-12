using Atlantic.Data.Contexts;
using Atlantic.Data.Models.Settings;
using Atlantic.Data.Models.System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Entities;

namespace Atlantic.Data.Services
{
    public interface IOtpService
    {
        Task<Otp> SendOtp(SendOtpRequest request);
        Task<Otp> Create(Otp country);
        Task<Otp> Remove(Otp country);
        Task<Otp> GetByEmail(string email);
        bool IsValid(Otp otp);

    }
    public class OtpService : IOtpService
    {
        protected readonly MongoContext _context;
        private readonly IMailService _mailService;
        private string _siteName;
        private string _siteUrl;
        private string _siteEmail;
        private readonly IHostEnvironment _hostEnvironment;
        public OtpService(IMailService ImailService, IOptions<DbSettings> options, IHostEnvironment hostEnvironment)
        {
            _context = new MongoContext();
            _mailService = ImailService;
            _siteName = options.Value.SiteName;
            _siteUrl = options.Value.SiteLink;
            _siteEmail = options.Value.DefaultEmail;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<Otp> Create(Otp otp)
        {

            await _context.SaveAsync(otp);
            return otp;
        }

        public async Task<Otp> GetByEmail(string email)
        {
            return await _context.Find<Otp>().Match(a => a.Email == email).Sort(s => s.CreatedDate, Order.Descending).Limit(1).ExecuteFirstAsync();
        }

        public async Task<Otp> Remove(Otp otp)
        {
            await _context.DeleteAsync<Otp>(otp.Email);
            return otp;
        }

        public async Task<Otp> SendOtp(SendOtpRequest request)
        {
            //var getOtp = await GetByEmail(request.Email);
            var specificTime = DateTime.Now.AddMinutes(10);
            Otp opt = new Otp();
            opt.Email = request.Email;
            if (_hostEnvironment.IsDevelopment())
            {
                opt.Token = "111111";
            }
            else
            {
                opt.Token = GenerateOtp();
            }

            opt.Validity = DateTime.SpecifyKind(specificTime, DateTimeKind.Utc); // 5 minutes
            var _Otp = await Create(opt);
            // send email  
            string mailBody =
                 $"Greetings from {_siteName}. " +
                 $"<br>Use this OTP to login " +
                 $"<br>Your OTP is: {_Otp.Token}. Do not share it with anyone." +
                 $"<br>Thank you for choosing {_siteName}";
            if (request.Email.ToLower() == "admin@example.com")
            {
                request.Email = _siteEmail;
            }

            if (!_hostEnvironment.IsDevelopment())
            {
                await _mailService.
                    SendEmailAsync(new MailRequest
                    {
                        ToEmail = request.Email,
                        Name = request.Email,
                        Subject = "OTP (Please do not share with anyone)",
                        Body = mailBody
                    });
            }
            else
            {
                Console.WriteLine(mailBody);
            }
            return _Otp;

        }
        public bool IsValid(Otp otp)
        {
            if (otp != null)
            {
                // check otp is available 
                var now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                TimeSpan diff = now.Subtract(otp.Validity);
                if (diff.TotalMinutes < 1)
                {
                    // didn't exceed 5 minutes
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        private string GenerateOtp()
        {
            Random generator = new Random();
            String r = generator.Next(0, 1000000).ToString("D6");
            return r;
        }
    }
}

using Atlantic.Data.Models.Settings;
using Atlantic.Data.Repositories;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Atlantic.Data.Services
{
    public interface IMailService
    {
        //Task SendEmailAsync(ApprovalPolicy mailRequest);
        //Task SendEmailAsync(VendorOnboardingMailRequest mailRequest);
        Task SendWelcomeEmailAsync(WelcomeRequest request);
        Task SendEmailAsync(MailRequest request);
        Task SendTicketEmailAsync(MailRequest reques);
    }
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        private string _siteEmail;
        private IUnitOfWork _unitOfWork;
        public MailService(IOptions<MailSettings> mailSettings, IUnitOfWork unitOfWork, IOptions<DbSettings> options)
        {
            _unitOfWork = unitOfWork;
            _mailSettings = mailSettings.Value;
            _siteEmail = options.Value.DefaultEmail;
        }

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.From.Add(InternetAddress.Parse(_mailSettings.Mail));
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));

            //email.Bcc.Add(InternetAddress.Parse("zia@panacea.com.my"));
            //email.Bcc.Add(InternetAddress.Parse("rronyy@gmail.com"));

            email.Subject = mailRequest.Subject;
            var builder = new BodyBuilder();
            if (mailRequest.Attachments != null)
            {
                foreach (var file in mailRequest.Attachments)
                {
                    builder.Attachments.Add(file);
                }
            }
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            try
            {
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.Auto);
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task SendTicketEmailAsync(MailRequest request)
        {
            string FilePath = Directory.GetCurrentDirectory() + "\\MailTemplates\\TicketTemplate.html";
            StreamReader str = new StreamReader(FilePath);
            string MailText = str.ReadToEnd();
            str.Close();
            var email = new MimeMessage();
            email.Subject = request.Subject;
            email.From.Add(InternetAddress.Parse(_mailSettings.Mail));
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(request.ToEmail));
            var builder = new BodyBuilder();
            builder.HtmlBody = MailText;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            try
            {
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.Auto);
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        public async Task SendWelcomeEmailAsync(WelcomeRequest request)
        {
            string FilePath = Directory.GetCurrentDirectory() + "\\MailTemplates\\WelcomeTemplate.html";
            StreamReader str = new StreamReader(FilePath);
            string MailText = str.ReadToEnd();
            str.Close();
            MailText = MailText.Replace("[username]", request.UserName).Replace("[email]", request.ToEmail);
            var email = new MimeMessage();
            email.From.Add(InternetAddress.Parse(_mailSettings.Mail));
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(request.ToEmail));
            email.Subject = $"Welcome {request.UserName}";
            var builder = new BodyBuilder();
            builder.HtmlBody = MailText;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            try
            {
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.Auto);
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }

    public class WelcomeRequest
    {
        public string ToEmail { get; set; }
        public string UserName { get; set; }
    }



    public class MailRequest
    {
        public string ToEmail { get; set; }
        public string? Name { get; set; }
        public string Subject { get; set; }
        public string? Body { get; set; }
        public List<string> Attachments { get; set; } = new List<string>();
    }
}

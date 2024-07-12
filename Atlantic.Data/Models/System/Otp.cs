using Atlantic.Data.Audit;

namespace Atlantic.Data.Models.System
{
    public class Otp : AuditableEntity
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public DateTime Validity { get; set; }
    }

    public class VerfifyUser
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Otp { get; set; }

    }
}

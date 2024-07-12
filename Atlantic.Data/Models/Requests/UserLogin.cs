namespace DMS.Data.Models.Requests
{
    public class UserLogin
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Otp { get; set; }
    }
}

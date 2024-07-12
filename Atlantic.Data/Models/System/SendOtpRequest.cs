namespace Atlantic.Data.Models.System
{
    public class SendOtpRequest
    {
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}

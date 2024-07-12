using MongoDB.Entities;

namespace Atlantic.Data.Models.Auth
{
    public class AuthToken : Entity
    {
        public string? UserId { get; set; }
        public string? Token { get; set; }
        public DateTime IssuedAt { get; set; } = DateTime.Now;
        public DateTime ExpiresAt { get; set; }
    }
}

using MongoDB.Entities;

namespace Atlantic.Data.Models.Auth
{
    public class UserConnection : Entity
    {
        public string ConnectionId { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public DateTime ConnectedAt { get; set; } = DateTime.Now;
    }
}

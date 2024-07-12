using Atlantic.Data.Models.Auth;
using MongoDB.Entities;

namespace Atlantic.Data.Services
{
    public interface IConnectionService
    {
        public Task Connected(string connectionId, string userId, string username = "");
        public Task Disconnected(string userId);
    }
    public class ConnectionService : IConnectionService
    {

        public async Task Connected(string connectionId, string userId, string username = "")
        {
            await Disconnected(userId);
            await Disconnected(connectionId);
            var conn = new UserConnection
            {
                ConnectionId = connectionId,
                UserId = userId,
                Username = username
            };
            await conn.SaveAsync();
        }

        public async Task Disconnected(string userId)
        {
            var connections = await DB.Find<UserConnection>().ManyAsync(f => f.UserId == userId || f.ConnectionId == userId);
            await connections.DeleteAllAsync();
        }
    }


}

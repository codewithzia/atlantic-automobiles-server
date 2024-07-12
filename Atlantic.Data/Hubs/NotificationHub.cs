using Atlantic.Data.Services;
using Microsoft.AspNetCore.SignalR;

namespace Atlantic.Data.Hubs
{
    public class NotificationHub : Hub
    {
        private IConnectionService _connection;
        private readonly IHttpContextAccessorService _httpAccessor;

        public NotificationHub(IConnectionService connectionService, IHttpContextAccessorService httpAccessor)
        {
            _connection = connectionService;
            _httpAccessor = httpAccessor;
        }
        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
        public override Task OnConnectedAsync()
        {
            _connection.Connected(Context.ConnectionId, _httpAccessor.CurrentUserId());
            Clients.Client(Context.ConnectionId).SendAsync("receiveMessage", new { ConnectionId = Context.ConnectionId });
            return base.OnConnectedAsync();
        }

        public Task OnDisconnectedAsync()
        {
            _connection.Disconnected(Context.ConnectionId);
            var ex = new Exception();
            return base.OnDisconnectedAsync(ex);
        }
        public async Task SendMessage(string user, string message, List<string> clients)
        {
            await Clients.Clients(clients).SendAsync("Message", message);
        }
        public async Task SendMessageToAll(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}

using Microsoft.AspNetCore.SignalR;

namespace PaymentService.Hubs
{
    public class NotificationHub : Hub
    {
        public static readonly string CONNECTED = "Connected";
        public static readonly string DISCONNECTED = "Disconnected";

        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            Console.WriteLine($"Client connected: {connectionId}");

            await base.OnConnectedAsync();

            if (Context.User.Identity.IsAuthenticated)
            {
                var userIdentifier = Context.UserIdentifier;
                await Clients.Caller.SendAsync(CONNECTED, connectionId, $"Connected to the server as {userIdentifier}");
            }
            else
            {
                await Clients.Caller.SendAsync(CONNECTED, connectionId, "Connected to the server");
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;
            Console.WriteLine($"Client disconnected: {connectionId}");

            await base.OnDisconnectedAsync(exception);
            await Clients.Caller.SendAsync(DISCONNECTED, connectionId, "Disconnected from the server");
        }
    }
}
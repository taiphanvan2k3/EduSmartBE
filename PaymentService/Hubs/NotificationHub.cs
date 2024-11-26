using Microsoft.AspNetCore.SignalR;

namespace PaymentService.Hubs
{
    public class NotificationHub : Hub
    {
        public static readonly string CONNECTED = "Connected";
        public static readonly string DISCONNECTED = "Disconnected";
    }
}
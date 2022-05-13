
using Microsoft.AspNetCore.SignalR;

namespace BulkyBookWeb.Hubs
{
    public class NotificationHub:Hub
    {
        
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}

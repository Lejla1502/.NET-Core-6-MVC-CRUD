using Microsoft.AspNetCore.SignalR;

namespace BulkyBookWeb.Hubs
{
    public class ChatHub:Hub
    {
        public async Task SendMessageToAll(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}

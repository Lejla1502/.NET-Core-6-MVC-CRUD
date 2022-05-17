
using Microsoft.AspNetCore.SignalR;

namespace BulkyBookWeb.Hubs
{
    public class NotificationHub:Hub
    {

        //public async Task Invole(string user, string message)
        //{
        //    await Clients.All.("ReceiveMessage", user, message);
        //}
        //public static async Task TaskCompleted(IHubContext<ChatHub> hubContext, int id)
        //{
        //    await hubContext.Clients.All.InvokeAsync("Completed", id);
        //}
    }
}

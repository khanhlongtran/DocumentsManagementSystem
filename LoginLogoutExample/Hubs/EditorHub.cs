using Microsoft.AspNetCore.SignalR;

namespace LoginLogoutExample.Hubs
{
    public class EditorHub : Hub
    {
        public async Task SendTextUpdate(string newText)
        {
            await Clients.All.SendAsync("ReceiveTextUpdate", newText);
        }
    }
}

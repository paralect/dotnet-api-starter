using Microsoft.AspNetCore.SignalR;

namespace SignalR.Hubs
{
    public class UserHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("connect");
        }

        public async Task Subscribe(string groupName)
        {
            if (HasAccessToRoom(groupName))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            }
        }

        private bool HasAccessToRoom(string groupName)
        {
            return groupName == $"user-{Context.User.Identity.Name}";
        }
    }
}
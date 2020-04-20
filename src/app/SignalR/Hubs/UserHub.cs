using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SignalR.Hubs
{
    public class UserHub : Hub
    {
        public async Task Subscribe(string roomId)
        {
            if (HasAccessToRoom(roomId))
            {

            }
        }

        private bool HasAccessToRoom(string roomId)
        {
            var roomData = roomId.Split('-');
            var roomType = roomData[0];
            var id = roomData[1];

            return Context.User.Identity.Name == id;
        }
    }
}
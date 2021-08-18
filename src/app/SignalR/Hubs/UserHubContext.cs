using System.Threading.Tasks;
using AutoMapper;
using Common.Models;
using Microsoft.AspNetCore.SignalR;
using SignalR.Models;

namespace SignalR.Hubs
{
    public class UserHubContext : IUserHubContext
    {
        private readonly IHubContext<UserHub> _hubContext;
        private readonly IMapper _mapper;

        public UserHubContext(IHubContext<UserHub> hubContext, IMapper mapper)
        {
            _hubContext = hubContext;
            _mapper = mapper;
        }

        public async Task SendUpdateAsync(IUser user)
        {
            var groupName = $"user-{user.Id}";
            await _hubContext.Clients.Group(groupName).SendAsync("user:updated", _mapper.Map<UserViewModel>(user));
        }
    }
}

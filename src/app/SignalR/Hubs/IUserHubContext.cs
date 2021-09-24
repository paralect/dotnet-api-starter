using System.Threading.Tasks;
using Common.Models;

namespace SignalR.Hubs
{
    public interface IUserHubContext
    {
        Task SendUpdateAsync(IUser user);
    }
}

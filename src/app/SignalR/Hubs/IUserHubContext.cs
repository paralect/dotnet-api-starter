using System.Threading.Tasks;
using Common.DAL.Documents.User;

namespace SignalR.Hubs
{
    public interface IUserHubContext
    {
        Task SendUpdateAsync(User user);
    }
}

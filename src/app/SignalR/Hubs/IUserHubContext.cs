using System.Threading.Tasks;
using Common.DAL.Documents;

namespace SignalR.Hubs
{
    public interface IUserHubContext
    {
        Task SendUpdateAsync(User user);
    }
}

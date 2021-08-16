using System.Threading.Tasks;
using Common.DB.Postgres.DAL.Documents;

namespace SignalR.Hubs
{
    public interface IUserHubContext
    {
        Task SendUpdateAsync(User user);
    }
}

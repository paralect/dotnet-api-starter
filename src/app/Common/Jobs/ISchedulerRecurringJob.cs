using System.Threading.Tasks;

namespace Common.Jobs;

public interface ISchedulerRecurringJob
{
    Task ExecuteAsync();
}
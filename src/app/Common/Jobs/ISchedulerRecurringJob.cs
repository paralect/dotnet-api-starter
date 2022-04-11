using System.Threading.Tasks;

namespace Common.Jobs;

public interface ISchedulerRecurringJob : ISchedulerJob
{
    Task Execute();
}
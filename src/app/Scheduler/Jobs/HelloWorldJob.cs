using Common.Jobs;

namespace Scheduler.Jobs;

public class HelloWorldJob : IHelloWorldJob
{
    public async Task ExecuteAsync()
    {
        Console.WriteLine("Hello world");
        await Task.CompletedTask;
    }
}
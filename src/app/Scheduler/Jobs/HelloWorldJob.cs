using Common.Jobs;

namespace Scheduler.Jobs;

public class HelloWorldJob : IHelloWorldJob
{
    public async Task Execute()
    {
        Console.WriteLine("Hello world");
        await Task.CompletedTask;
    }
}
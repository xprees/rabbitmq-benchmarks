namespace Benchmarking.Benchmarks;

[MarkdownExporter]
[MinColumn, MaxColumn, MeanColumn, MedianColumn]
public class MassTransitBenchmark
{
    [GlobalSetup]
    public void Setup()
    {
    }

    [GlobalCleanup]
    public void Cleanup()
    {
    }

    [IterationSetup]
    public void IterationSetup()
    {
    }


    // TODO compare RabbitMQ client against MassTransit
}
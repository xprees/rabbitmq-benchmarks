using Messaging.RabbitMQ;
using Messaging.RabbitMQ.MassTransit;

namespace Benchmarking.Benchmarks;

[MarkdownExporter]
[MinColumn, MaxColumn, MeanColumn, MedianColumn]
public class MassTransitBenchmark
{
    private MassTransitHelper _massTransitHelper = null!;

    [GlobalSetup]
    public void Setup()
    {
        _massTransitHelper = new MassTransitHelper();
        _massTransitHelper.Start();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _massTransitHelper?.Dispose();
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _massTransitHelper.Consumer.Reset();
    }


    [Benchmark(Description = "MassTransit", OperationsPerInvoke = 100000)]
    public async Task PublishMessage()
    {
        await _massTransitHelper.PublishMessage(new TestRequest("Test"));
        await _massTransitHelper.Consumer.WaitForMessage();
    }
}
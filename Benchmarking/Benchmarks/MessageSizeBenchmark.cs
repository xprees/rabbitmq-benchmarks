using BenchmarkDotNet.Attributes;

namespace Receiver.Benchmarks;

// TODO compare in different broker configurations and setups (federate)
// TODO connect grafana to the broker and monitor the performance
// TODO compare different message brokers configurations (federate, cluster, etc.)
// TODO compare different message sizes (1000, 10000, 100000)
// TODO compare different message brokers setups (single node, multiple nodes, etc.)
public class MessageSizeBenchmark
{
    private string _message;

    [Params(1000, 10000, 100000)] public int messageSize;

    [GlobalSetup]
    public void Setup()
    {
        var message = 0x222;
    }

    [Benchmark]
    public void SendMessage()
    {
        // TODO send message
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        // TODO close connection
    }
}
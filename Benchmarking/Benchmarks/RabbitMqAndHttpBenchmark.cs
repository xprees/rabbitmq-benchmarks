using System.Net.Http.Json;
using BenchmarkDotNet.Order;
using Messaging.RabbitMQ;
using RabbitMQ.Client.Events;

namespace Benchmarking.Benchmarks;

[RPlotExporter]
[MarkdownExporter]
[Orderer(SummaryOrderPolicy.Declared)]
[MinColumn, MaxColumn, MeanColumn, MedianColumn]
public class RabbitMqAndHttpBenchmark
{
    private RabbitTestingHelper _sendOnlyRabbitHelper = null!;
    private RabbitTestingHelper _sendRabbitHelper = null!;
    private RabbitTestingHelper _receiveRabbitHelper = null!;

    private const string RestApiBaseUrl = "http://localhost:5050";

    private HttpClient _httpClient = null!;

    // 1KB, 100KB, 512KB, 1MB, 8MB, 32MB, 64MB, 100MB
    //[Params(1024, 1024 * 100, 1024 * 512, 1_048_576, 8_388_608, 33_554_432, 67_108_864, 104_857_600)]
    [Params(1024, 1024 * 100)]
    public int MessageSize { get; set; } // (128MB max message size RabbitMQ supports, 100MB for HTTP)

    private byte[] _message = null!;
    private TaskCompletionSource<bool> _messageReceivedSource = null!;

    [GlobalSetup]
    public void Setup()
    {
        _message = new byte[MessageSize];

        _sendOnlyRabbitHelper =
            new RabbitTestingHelper(RabbitConstants.SendOnlyQueue, onAck: (sender, args) => { });

        _sendRabbitHelper = new RabbitTestingHelper(RabbitConstants.SendQueue);
        _receiveRabbitHelper =
            new RabbitTestingHelper(RabbitConstants.SendQueue, onMessageReceived: OnConsumerOnReceived);
        _httpClient = PrepareHttpClient(RestApiBaseUrl);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _sendOnlyRabbitHelper?.Channel?.QueuePurge(RabbitConstants.SendOnlyQueue); // No one is listening to this queue
        _sendOnlyRabbitHelper?.Dispose();
        _sendRabbitHelper?.Dispose();
        _receiveRabbitHelper?.Dispose();
        _httpClient?.Dispose();
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = 100, Description = "HTTP")]
    public async Task<HttpResponseMessage> MessageRestApi() =>
        await _httpClient.PostAsJsonAsync("receive", new { Data = _message });

    [Benchmark(OperationsPerInvoke = 100, Description = "RabbitMQ")]
    public bool? MessageRabbitOneWay()
    {
        _sendOnlyRabbitHelper.SendMessage(_message);
        var received = _sendOnlyRabbitHelper.Channel.WaitForConfirms(TimeSpan.FromSeconds(10));
        return received ? true : null; // null means discard the result - something went wrong
    }

    // This is a 2-way communication, so we need to wait for the reply to get from the broker (receiver also this client)
    [Benchmark(OperationsPerInvoke = 100, Description = "RabbitMQ-RT")]
    public async Task MessageRabbitWaitForReply()
    {
        _messageReceivedSource = new TaskCompletionSource<bool>(); // Reset the source
        _sendRabbitHelper.SendMessage(_message);

        // Wait for the reply
        await _messageReceivedSource.Task
            .ConfigureAwait(false);
    }

    private void OnConsumerOnReceived(object? model, BasicDeliverEventArgs ea) =>
        _messageReceivedSource?.SetResult(true);

    private static HttpClient PrepareHttpClient(string apiBaseUrl)
    {
        var client = new HttpClient(new HttpClientHandler
        {
            MaxRequestContentBufferSize = 134_217_728, // 128MB
        });

        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromMinutes(5);
        return client;
    }
}
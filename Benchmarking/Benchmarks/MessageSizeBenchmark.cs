using System.Net.Http.Json;
using Messaging.RabbitMQ;
using RabbitMQ.Client.Events;

namespace Benchmarking.Benchmarks;

[MarkdownExporter]
[RPlotExporter]
public class MessageSizeBenchmark
{
    private const string SendQueue = "sendQueue";
    private const string ReceiveQueue = "receiveQueue";

    private RabbitTestingHelper _sendTestingHelper = null!;
    private RabbitTestingHelper _receiveTestingHelper = null!;

    private const string RestApiBaseUrl = "http://localhost:5050";

    private HttpClient _httpClient = null!;

    // 512B, 1KB, 512KB, 1MB, 8MB, 32MB, 64 MB, 128MB (max message size)
    [Params(512, 1024, 1024 * 512, 1_048_576, 8_388_608, 33_554_432, 67_108_864, 134_217_728)]
    public int MessageSize;

    private byte[] _message = null!;
    private TaskCompletionSource<bool> _messageReceivedSource = null!;

    [GlobalSetup]
    public void Setup()
    {
        _message = new byte[MessageSize];
        _sendTestingHelper = new RabbitTestingHelper(SendQueue);
        _receiveTestingHelper = new RabbitTestingHelper(ReceiveQueue, onMessageReceived: OnConsumerOnReceived);
        _httpClient = PrepareHttpClient(RestApiBaseUrl);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _sendTestingHelper?.Dispose();
        _receiveTestingHelper?.Dispose();
        _httpClient?.Dispose();
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _messageReceivedSource = new TaskCompletionSource<bool>();
    }

    [Benchmark(Baseline = true)]
    public async Task<HttpResponseMessage> MessageRestApi() =>
        await _httpClient.PostAsJsonAsync("receive", new { Data = _message });

    [Benchmark]
    public async Task MessageRabbitWaitForReply()
    {
        _sendTestingHelper.SendMessage(_message);

        // Wait for the reply
        await _messageReceivedSource.Task
            .ConfigureAwait(false);
    }

    private void OnConsumerOnReceived(object? model, BasicDeliverEventArgs ea)
    {
        _messageReceivedSource.SetResult(true);
    }

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
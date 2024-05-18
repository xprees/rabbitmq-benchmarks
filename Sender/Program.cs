using System.Net.Http.Json;
using Messaging.RabbitMQ;
using RabbitMQ.Client.Events;

await CallRestApiEchoEndpoint("echo", new byte[104_857_601]); // 100MB max message size for System.Text.Json

return;

async Task CallRestApiEchoEndpoint(string endpoint = "echo", object? data = null)
{
    using var httpClient = PrepareHttpClient("http://localhost:5050");

    var result = await httpClient.PostAsJsonAsync(endpoint, new { Data = data ?? "Test" });
    var content = await result.Content.ReadAsStringAsync();
    Console.WriteLine($"{result.StatusCode} - {content[..100]}");
}

static HttpClient PrepareHttpClient(string apiBaseUrl)
{
    var client = new HttpClient(new HttpClientHandler
    {
        MaxRequestContentBufferSize = 134_217_728, // 128MB
        MaxResponseHeadersLength = 134_217_728, // 128MB
    });

    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromMinutes(5);
    return client;
}

void StartRabbitPingPong()
{
    const string sendQueue = "sendQueue"; // It's the opposite of the other project
    const string receiveQueue = "receiveQueue";

    using var callbackClient = new RabbitTestingHelper(sendQueue);
    using var receiverClient =
        new RabbitTestingHelper(receiveQueue, onMessageReceived: OnMessageReceived(callbackClient));

    Console.WriteLine($" Echoing messages from {receiveQueue} to {sendQueue} RabbitMQ queues.");

    Console.WriteLine(" ...and Kickoff!");
    callbackClient.SendMessage("Start"u8.ToArray());


    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();

    callbackClient.Channel.QueuePurge(sendQueue);
    receiverClient.Channel.QueuePurge(receiveQueue);
}

EventHandler<BasicDeliverEventArgs> OnMessageReceived(RabbitTestingHelper rabbitTestingHelper) =>
    (_, ea) =>
    {
        var body = ea.Body.ToArray();
        rabbitTestingHelper.SendMessage(body);
    };
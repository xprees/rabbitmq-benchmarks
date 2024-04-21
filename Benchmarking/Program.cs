using System.Text;
using BenchmarkDotNet.Running;
using Benchmarking.Benchmarks;
using Messaging.RabbitMQ;
using RabbitMQ.Client.Events;

BenchmarkRunner.Run<MessageSizeBenchmark>();

return;
const string SendQueue = "sendQueue"; // It's the opposite of the other project
const string ReceiveQueue = "receiveQueue";

var tcs = new TaskCompletionSource<bool>();

using var callbackClient = new RabbitTestingHelper(SendQueue);
using var receiverClient = new RabbitTestingHelper(ReceiveQueue, onMessageReceived: OnMessageReceived(callbackClient));

callbackClient.SendMessage("Hello, World!"u8.ToArray());

await tcs.Task;

Console.WriteLine("Completed!");

callbackClient.Channel.QueuePurge(SendQueue);
receiverClient.Channel.QueuePurge(ReceiveQueue);

return;

EventHandler<BasicDeliverEventArgs> OnMessageReceived(RabbitTestingHelper rabbitTestingHelper) =>
    (_, ea) =>
    {
        tcs.SetResult(true);
        return;

        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        var backMessage = Encoding.UTF8.GetBytes(message);
        rabbitTestingHelper.SendMessage(backMessage);
    };
#undef ECHO

using Messaging.RabbitMQ;
using RabbitMQ.Client.Events;

const string sendQueue = "sendQueue"; // It's the opposite of the other project
const string receiveQueue = "receiveQueue";

using var callbackClient = new RabbitTestingHelper(receiveQueue);
using var receiverClient = new RabbitTestingHelper(sendQueue, onMessageReceived: OnMessageReceived);

Console.WriteLine($" Echoing messages from {sendQueue} to {receiveQueue} RabbitMQ queues.");
Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();

return;

void OnMessageReceived(object? model, BasicDeliverEventArgs ea)
{
#if ECHO
    var body = ea.Body.ToArray();
    callbackClient.SendMessage(body);
#else
    callbackClient.SendMessage([]);
#endif
}
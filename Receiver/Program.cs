#undef ECHO

using Messaging.RabbitMQ;
using RabbitMQ.Client.Events;

using var callbackClient = new RabbitTestingHelper(RabbitConstants.ReceiveQueue);
using var receiverClient = new RabbitTestingHelper(RabbitConstants.SendQueue, onMessageReceived: OnMessageReceived);

Console.WriteLine(
    $" Echoing messages from {RabbitConstants.SendQueue} to {RabbitConstants.ReceiveQueue} RabbitMQ queues.");
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
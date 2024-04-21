using System.Text;
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
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    var backMessage = Encoding.UTF8.GetBytes(message);
    callbackClient.SendMessage(backMessage);
#if DEBUG
        Console.WriteLine("Message received and sent back");
#endif
}

;
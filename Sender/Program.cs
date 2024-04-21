using Messaging.RabbitMQ;
using RabbitMQ.Client.Events;

const string sendQueue = "sendQueue"; // It's the opposite of the other project
const string receiveQueue = "receiveQueue";

using var callbackClient = new RabbitTestingHelper(sendQueue);
using var receiverClient = new RabbitTestingHelper(receiveQueue, onMessageReceived: OnMessageReceived(callbackClient));

Console.WriteLine($" Echoing messages from {receiveQueue} to {sendQueue} RabbitMQ queues.");

Console.WriteLine(" ...and Kickoff!");
callbackClient.SendMessage("Start"u8.ToArray());


Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();

callbackClient.Channel.QueuePurge(sendQueue);
receiverClient.Channel.QueuePurge(receiveQueue);

return;

EventHandler<BasicDeliverEventArgs> OnMessageReceived(RabbitTestingHelper rabbitTestingHelper) =>
    (_, ea) =>
    {
        var body = ea.Body.ToArray();
        rabbitTestingHelper.SendMessage(body);
    };
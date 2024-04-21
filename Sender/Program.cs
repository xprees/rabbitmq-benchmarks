using Messaging.RabbitMQ;

using var client = new RabbitTestingHelper("sendQueue");

var message = "Hello Igor!"u8.ToArray();

while (true)
{
    client.SendMessage(message);
    Console.WriteLine($" [x] Sent {message.Length} bytes: {message}");

    Console.WriteLine("Send next message");
    if (Console.ReadLine() == "q") break;
}


Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();
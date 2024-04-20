using System.Text;
using Messaging.RabbitMQ;
using RabbitMQ.Client.Events;

using var client = new RabbitTestingHelper("localhost", "way-1", OnMessageReceived());

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();

return;

EventHandler<BasicDeliverEventArgs>? OnMessageReceived() =>
    (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine($" [x] Received {message}");
    };
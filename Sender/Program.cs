using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "hello3",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

var message = "Hello Igor!"u8.ToArray();

while (true)
{
    channel.BasicPublish(exchange: string.Empty,
        routingKey: "hello",
        basicProperties: null,
        body: message
    );
    Console.WriteLine($" [x] Sent {message.Length} bytes: {message}");

    Console.WriteLine("Send next message");
    if (Console.ReadLine() == "q") break;
}


Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();
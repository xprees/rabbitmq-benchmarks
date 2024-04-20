using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Messaging.RabbitMQ;

public class RabbitTestingHelper : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly EventingBasicConsumer? _consumer;
    private readonly EventHandler<BasicDeliverEventArgs>? _onMessageReceived;

    public string QueueName { get; }
    public IModel Channel => _channel;

    public RabbitTestingHelper(string hostName, string queueName,
        EventHandler<BasicDeliverEventArgs>? onMessageReceived = null)
    {
        _onMessageReceived = onMessageReceived;
        QueueName = queueName;

        var factory = new ConnectionFactory { HostName = hostName };
        _connection = factory.CreateConnection();
        _channel = PrepareChannel();

        if (onMessageReceived != null)
        {
            _consumer = StartReceiving(onMessageReceived);
        }
    }

    private IModel PrepareChannel()
    {
        var channel = _connection.CreateModel();
        channel.QueueDeclare(
            queue: QueueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
        return channel;
    }

    private EventingBasicConsumer StartReceiving(EventHandler<BasicDeliverEventArgs> onMessageReceived)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += onMessageReceived;
        _channel.BasicConsume(
            queue: QueueName,
            autoAck: true,
            consumer: consumer
        );

        return consumer;
    }

    public void SendMessage(byte[] message) =>
        _channel.BasicPublish(
            exchange: string.Empty,
            routingKey: QueueName,
            body: message
        );


    public void Dispose()
    {
        if (_onMessageReceived != null && _consumer != null) _consumer.Received -= _onMessageReceived;
        _connection.Dispose();
        _channel.Dispose();
    }
}
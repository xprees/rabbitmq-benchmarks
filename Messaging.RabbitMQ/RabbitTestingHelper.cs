using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Messaging.RabbitMQ;

public class RabbitTestingHelper : IDisposable
{
    private readonly IConnection _connection;
    private readonly EventingBasicConsumer? _consumer;
    private readonly EventHandler<BasicDeliverEventArgs>? _onMessageReceived;

    public string QueueName { get; }
    public IModel Channel { get; }

    public RabbitTestingHelper(string queueName, string hostName = "localhost",
        EventHandler<BasicDeliverEventArgs>? onMessageReceived = null,
        EventHandler<BasicAckEventArgs>? onAck = null,
        EventHandler<BasicNackEventArgs>? onNack = null)
    {
        _onMessageReceived = onMessageReceived;
        QueueName = queueName;

        var factory = new ConnectionFactory { HostName = hostName };
        _connection = factory.CreateConnection();
        Channel = PrepareChannel();

        if (onAck is not null || onNack is not null)
        {
            Channel.ConfirmSelect();
            if (onAck != null) Channel.BasicAcks += onAck;
            if (onNack != null) Channel.BasicNacks += onNack;
        }

        if (onMessageReceived != null) _consumer = StartReceiving(onMessageReceived);
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
        var consumer = new EventingBasicConsumer(Channel);
        consumer.Received += onMessageReceived;
        Channel.BasicConsume(
            queue: QueueName,
            autoAck: true,
            consumer: consumer
        );

        return consumer;
    }

    public void SendMessage(byte[] message) =>
        Channel.BasicPublish(
            exchange: string.Empty,
            routingKey: QueueName,
            body: new ReadOnlyMemory<byte>(message));


    public void Dispose()
    {
        if (_onMessageReceived != null && _consumer != null) _consumer.Received -= _onMessageReceived;
        Channel.Close();
        _connection.Close();
        _connection.Dispose();
        Channel.Dispose();
    }
}
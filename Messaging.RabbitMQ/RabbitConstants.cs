namespace Messaging.RabbitMQ;

public static class RabbitConstants
{
    public const string SendQueue = "send";
    public const string SendOnlyQueue = "send-ack-only";
    public const string ReceiveQueue = "receive";
}
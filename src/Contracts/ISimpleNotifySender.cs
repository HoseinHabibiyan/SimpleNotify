namespace SimpleNotify.Contracts;

public interface ISimpleNotifySender
{
    ValueTask Publish<TNotification>(TNotification notification) where TNotification : INotification<TNotification>;
}
namespace SimpleNotify.Contracts;

public interface INotificationSender
{
    ValueTask Publish<TNotification>(TNotification notification) where TNotification : INotification<TNotification>;
}
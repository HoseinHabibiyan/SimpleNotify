namespace SimpleNotify.Contracts;

public interface INotificationHandler<in TNotification>
{
    ValueTask Handle(TNotification notification);
}
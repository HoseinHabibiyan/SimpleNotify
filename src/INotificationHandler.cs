namespace SimpleNotify;

public interface INotificationHandler<in TNotification>
{
    ValueTask Handle(TNotification notification);
}
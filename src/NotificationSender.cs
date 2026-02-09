using Microsoft.Extensions.DependencyInjection;
using SimpleNotify.Contracts;

namespace SimpleNotify;

public class NotificationSender(IServiceProvider serviceProvider) : INotificationSender
{
    public async ValueTask Publish<TNotification>(TNotification notification) where TNotification : INotification<TNotification>
    {
        var handlers =
            serviceProvider.GetServices<INotificationHandler<TNotification>>();

        foreach (var handler in handlers) 
            await handler.Handle(notification);
    }
}
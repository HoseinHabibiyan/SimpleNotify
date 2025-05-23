using SimpleNotify.Contracts;

namespace SimpleNotify;

public class SimpleNotifySender : ISimpleNotifySender
{
    public async ValueTask Publish<TNotification>(TNotification notification) where TNotification : INotification<TNotification>
    {
        List<Type> types = ServiceRegistrar.Assemblies.SelectMany(x => x.GetTypes())
            .Where(x => x.IsClass && x.IsAssignableTo(typeof(INotificationHandler<TNotification>))).ToList();

        foreach (var handler in types.Select(type => Activator.CreateInstance(type) as INotificationHandler<TNotification>))
        {
            await handler!.Handle(notification);
        }
    }
}
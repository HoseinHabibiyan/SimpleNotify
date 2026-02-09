using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SimpleNotify.Contracts;

namespace SimpleNotify;

public static class ServiceRegistrar
{
    public static void AddSimpleNotify(this IServiceCollection services, Assembly[] assembliesToScan)
        => services.RegisterHandlers(assembliesToScan);


    public static void AddSimpleNotify<TAssembly>(this IServiceCollection services) where TAssembly : class
    {
        var assembly = typeof(TAssembly).Assembly;
        services.RegisterHandlers([assembly]);
    }


    private static void RegisterHandlers(this IServiceCollection services, Assembly[] assembliesToScan)
    {
        if (assembliesToScan is null || !assembliesToScan.Any())
            throw new ArgumentException(
                $"[SimpleNotify::{nameof(AddSimpleNotify)}] Argument '{nameof(assembliesToScan)}' cannot be null or empty.");

        services.AddScoped<INotificationSender, NotificationSender>();
        assembliesToScan = assembliesToScan.Distinct().ToArray();

        var handlerInterface = typeof(INotificationHandler<>);

        var handlers = assembliesToScan
            .SelectMany(a => a.GetTypes())
            .Where(t =>
                t is { IsClass: true, IsAbstract: false } &&
                t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == handlerInterface));

        foreach (var handler in handlers)
        {
            var interfaces = handler.GetInterfaces()
                .Where(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == handlerInterface);

            foreach (var @interface in interfaces)
                services.AddScoped(@interface, handler);
        }
    }
}
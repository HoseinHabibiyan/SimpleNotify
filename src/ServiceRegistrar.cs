using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SimpleNotify.Contracts;

namespace SimpleNotify;

public static class ServiceRegistrar
{
    public static Assembly[] Assemblies = [];
    
    public static void AddSimpleNotify(this IServiceCollection services,params Assembly[] assembliesToScan)
    {
        if (assembliesToScan is null || !assembliesToScan.Any())
            throw new ArgumentException($"[SimpleNotify::{nameof(AddSimpleNotify)}] Argument '{nameof(assembliesToScan)}' cannot be null or empty.");
        
        services.AddScoped<ISimpleNotifySender, SimpleNotifySender>();
        Assemblies = assembliesToScan.Distinct().ToArray();
    }
}
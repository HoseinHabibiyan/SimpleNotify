# SimpleNotify

**SimpleNotify** is a lightweight and minimal notification library for .NET, inspired by the notification pattern in [MediatR](https://github.com/jbogard/MediatR).  
It allows you to **raise and handle notifications** (also known as events) across your application layers — without the overhead.

---

## ✨ Features

- 🟢 Lightweight, minimal API
- 🧩 Decoupled notification handling
- ⚡ Fast and dependency-free
- 🧠 Familiar `INotification` interface

---


## 📦 Installation

Install via NuGet:

```bash
dotnet add package SimpleNotify
```

Install via NuGet:

```bash
dotnet add package SimpleNotify
```




🚀 Usage
###  1. Register the service in Program.cs

```
builder.Services.AddSimpleNotify(typeof(Program).Assembly);
```

### 2. Define a Notification
```
public record AddOrderNotify(Guid OrderId) : INotification<AddOrderNotify>;
```

### 3. Create a Notification Handler
```
public class AddOrderNotificationHandler : INotificationHandler<AddOrderNotify>
{
    public ValueTask Handle(AddOrderNotify source)
    {
        Console.WriteLine($"Order {source.OrderId} has been added");
        return default;
    }
}
```

### 4. Inject and Publish the Notification
Inject the sender service:
```
private readonly ISimpleNotifySender _simpleNotifySender;
```

Publish the event:
```
Guid orderId = Guid.NewGuid();
await _simpleNotifySender.Publish(new AddOrderNotify(orderId));
```

## 🤝 Contribute
- Contributions are always welcome!
- Found a bug or have an idea?  
- Feel free to open an issue or submit a pull request.
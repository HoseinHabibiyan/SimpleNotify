using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using SimpleNotify.Contracts;

namespace SimpleNotify.UnitTest;

public class NotificationSenderTests
{
    [Fact]
    public async Task When_AddOrderNotification_Is_Published_Should_Invoke_Handler_As_Expected()
    {
        #region Arrange

        var builder = new StringBuilder();
        var writer = new StringWriter(builder);

        ServiceCollection serviceCollection = new();
        
        serviceCollection.AddSimpleNotify<NotificationSenderTests>();
        
        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

        NotificationSender notificationSender = new(serviceProvider);

        AddOrderNotification notification = new(Guid.NewGuid().ToString(), new Random().Next())
        {
            Writer = writer
        };

        #endregion

        #region Act

        await notificationSender.Publish(notification);

        #endregion

        #region Assert

        string result = builder.ToString();

        result.ShouldContain(notification.ToString());

        #endregion
    }

    [Fact]
    public async Task When_AddOrderNotification_Is_Published_And_We_Have_MultipleHandlers_Should_Invoke_AllHandlers_As_Expected()
    {
        #region Arrange

        var builder = new StringBuilder();
        var writer = new StringWriter(builder);

        ServiceCollection serviceCollection = new();
        
        serviceCollection.AddSimpleNotify<NotificationSenderTests>();
        
        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

        NotificationSender notificationSender = new(serviceProvider);

        OrderAddedNotification notification = new(Guid.NewGuid().ToString(), new Random().Next())
        {
            Writer = writer
        };

        #endregion

        #region Act

        await notificationSender.Publish(notification);

        #endregion

        #region Assert

        string result = builder.ToString();

        var expected = result.Split("\n")
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Substring(0, notification.ToString().Length)).ToList();

        expected.Count.ShouldBe(3);
        expected.ShouldAllBe(x => x.Substring(0, notification.ToString().Length) == notification.ToString());
        expected.GroupBy(x => x).Count().ShouldBe(1);

        #endregion
    }

    [Fact]
    public void When_AddSimpleNotify_NotPassed_AssembliesToScan_Argument_Then_Should_Throw_Argument_Exception()
    {
        ServiceCollection services = new();
        Should.Throw<ArgumentException>(() => services.AddSimpleNotify([]));
    }

    private record AddOrderNotification(string OrderNumber, int Quantity) : INotification<AddOrderNotification>
    {
        public TextWriter Writer { get; init; } = TextWriter.Null;
        
        public override string ToString()
            => $"{nameof(AddOrderNotification)} {{ OrderNumber = {OrderNumber}, Quantity = {Quantity} }}";
    }

    private record AddOrderNotificationHandler : INotificationHandler<AddOrderNotification>
    {
        public ValueTask Handle(AddOrderNotification notification)
        {
            notification.Writer.WriteLine($"{notification} :: {Guid.NewGuid()}");
            return ValueTask.CompletedTask;
        }
    }
    
    private record OrderAddedNotification(string OrderNumber, int Quantity) : INotification<OrderAddedNotification>
    {
        public TextWriter Writer { get; init; } = TextWriter.Null;
        
        public override string ToString()
            => $"{nameof(AddOrderNotification)} {{ OrderNumber = {OrderNumber}, Quantity = {Quantity} }}";
    }
    
    private record OrderAddedNotificationHandler1 : INotificationHandler<OrderAddedNotification>
    {
        public ValueTask Handle(OrderAddedNotification notification)
        {
            notification.Writer.WriteLine($"{notification} :: {Guid.NewGuid()}");
            return ValueTask.CompletedTask;
        }
    }
    
    private record OrderAddedNotificationHandler2: INotificationHandler<OrderAddedNotification>
    {
        public ValueTask Handle(OrderAddedNotification notification)
        {
            notification.Writer.WriteLine($"{notification} :: {Guid.NewGuid()}");
            return ValueTask.CompletedTask;
        }
    }
    
    private record OrderAddedNotificationHandler3 : INotificationHandler<OrderAddedNotification>
    {
        public ValueTask Handle(OrderAddedNotification notification)
        {
            notification.Writer.WriteLine($"{notification} :: {Guid.NewGuid()}");
            return ValueTask.CompletedTask;
        }
    }
}
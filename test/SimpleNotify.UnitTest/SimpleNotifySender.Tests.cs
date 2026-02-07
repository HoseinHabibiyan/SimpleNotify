using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using SimpleNotify.Contracts;

namespace SimpleNotify.UnitTest;

public class SimpleNotifySenderTests
{
    [Fact]
    public async Task When_AddOrderNotification_Is_Published_Should_Invoke_Handler_As_Expected()
    {
        #region Arrange

        var builder = new StringBuilder();
        var writer = new StringWriter(builder);

        ServiceCollection serviceCollection = new();
        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

        SimpleNotifySender simpleNotifySender = new(serviceProvider);

        AddOrderNotification orderNotification = new(Guid.NewGuid().ToString(), new Random().Next());

        serviceCollection.AddSimpleNotify<SimpleNotifySender>();

        INotificationHandler<AddOrderNotification> handler = new AddOrderNotificationHandler(writer);
        await handler.Handle(orderNotification);

        #endregion

        #region Act

        await simpleNotifySender.Publish(orderNotification);

        #endregion

        #region Assert

        string result = builder.ToString();

        result.ShouldContain(orderNotification.ToString());

        #endregion
    }

    [Fact]
    public async Task When_AddOrderNotification_Is_Published_And_We_Have_MultipleHandlers_Should_Invoke_AllHandlers_As_Expected()
    {
        #region Arrange

        var builder = new StringBuilder();
        var writer = new StringWriter(builder);

        ServiceCollection serviceCollection = new();
        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

        SimpleNotifySender simpleNotifySender = new(serviceProvider);

        serviceCollection.AddSimpleNotify<SimpleNotifySender>();

        AddOrderNotification orderNotification = new(Guid.NewGuid().ToString(), new Random().Next());

        INotificationHandler<AddOrderNotification> handler1 = new AddOrderNotificationHandler(writer);
        await handler1.Handle(orderNotification);

        INotificationHandler<AddOrderNotification> handler2 = new AddOrderNotificationHandler(writer);
        await handler2.Handle(orderNotification);

        INotificationHandler<AddOrderNotification> handler3 = new AddOrderNotificationHandler(writer);
        await handler3.Handle(orderNotification);

        #endregion

        #region Act

        await simpleNotifySender.Publish(orderNotification);

        #endregion

        #region Assert

        string result = builder.ToString();

        var expected = result.Split("\n")
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Substring(0, orderNotification.ToString().Length)).ToList();

        expected.Count.ShouldBe(3);
        expected.ShouldAllBe(x => x.Substring(0, orderNotification.ToString().Length) == orderNotification.ToString());
        expected.GroupBy(x => x).Count().ShouldBe(1);

        #endregion
    }

    [Fact]
    public void When_AddSimpleNotify_NotPassed_AssembliesToScan_Argument_Then_Should_Throw_Argument_Exception()
    {
        ServiceCollection services = new();
        Should.Throw<ArgumentException>(() => services.AddSimpleNotify([]));
    }

    private record AddOrderNotification(string OrderNumber, int Quantity) : INotification<AddOrderNotification>;

    private record AddOrderNotificationHandler(TextWriter Writer) : INotificationHandler<AddOrderNotification>
    {
        public ValueTask Handle(AddOrderNotification notification)
        {
            Writer.WriteLine($"{notification} :: {Guid.NewGuid()}");
            return ValueTask.CompletedTask;
        }
    }
}
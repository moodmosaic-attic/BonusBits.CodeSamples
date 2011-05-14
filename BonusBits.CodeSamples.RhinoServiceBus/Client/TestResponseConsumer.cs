using System;
using BonusBits.CodeSamples.Rhino.ServiceBus.Messages;
using Rhino.ServiceBus;

public sealed class TestResponseConsumer : ConsumerOf<TestResponse>
{
    private readonly IServiceBus m_bus;

    public TestResponseConsumer(IServiceBus bus)
    {
        m_bus = bus;
    }

    #region ConsumerOf<TestResponse> Members

    public void Consume(TestResponse response)
    {
        Console.WriteLine("[Back-end reply] UserId=" + response.UserId);
    }

    #endregion
}

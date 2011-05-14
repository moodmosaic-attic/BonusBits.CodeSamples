using BonusBits.CodeSamples.Rhino.ServiceBus.Messages;
using Rhino.ServiceBus;

namespace BonusBits.CodeSamples.Rhino.ServiceBus.Backend
{
    public sealed class TestRequestConsumer : ConsumerOf<TestRequest>
    {
        private readonly IServiceBus m_bus;

        public TestRequestConsumer(IServiceBus bus)
        {
            m_bus = bus;
        }

        #region ConsumerOf<TestRequest> Members

        public void Consume(TestRequest message)
        {
            m_bus.Reply(new TestResponse() { UserId = 5 });
        }

        #endregion
    }
}

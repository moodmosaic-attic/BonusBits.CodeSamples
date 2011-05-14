using System;

namespace BonusBits.CodeSamples.Rhino.ServiceBus.Messages
{
    [Serializable]
    public sealed class TestResponse
    {
        public Int32 UserId { get; set; }
    }
}

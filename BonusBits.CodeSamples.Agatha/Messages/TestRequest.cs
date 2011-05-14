using System;

namespace BonusBits.CodeSamples.Agatha.Messages
{
    public sealed class TestRequest : global::Agatha.Common.Request
    {
        public String UserName { get; set; }

        public String Password { get; set; }
    }
}

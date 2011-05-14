using Agatha.Common;
using Agatha.ServiceLayer;
using BonusBits.CodeSamples.Agatha.Backend.Domain;
using BonusBits.CodeSamples.Agatha.Messages;

namespace BonusBits.CodeSamples.Agatha.Backend
{
    public sealed class TestHandler : RequestHandler<TestRequest, TestResponse>
    {
        public TestHandler(IDatabase db)
        {
        }

        public override Response Handle(TestRequest request)
        {
            TestResponse response = CreateTypedResponse();
            response.UserId = 5;
            return response;
        }
    }
}

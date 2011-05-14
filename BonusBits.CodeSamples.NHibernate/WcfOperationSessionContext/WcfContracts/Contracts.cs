using System.ServiceModel;

namespace WcfContracts
{
    [ServiceContract]
    public interface ICurrentSessionContextTestService
    {
        [OperationContract(IsOneWay = true)]
        void RunTests();
    }
}

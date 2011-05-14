using System;
using System.ServiceModel;
using WcfContracts;

namespace WcfClient
{
    internal sealed class Program
    {
        private static void Main()
        {
            Console.WriteLine("Press any key when server is ready..");
            Console.ReadKey(false);

            var proxy = new ChannelFactory<ICurrentSessionContextTestService>(
                new NetTcpBinding(), "net.tcp://localhost:56789");
            ICurrentSessionContextTestService svc = proxy.CreateChannel();

            // Start our loop.
            Char operation = (Char)0;
            while (operation != 'Q')
            {
                Console.WriteLine("R=Run Tests, Q=Quit?");
                operation = Char.ToUpper(Console.ReadKey(true).KeyChar);
                if (operation == 'R') { svc.RunTests(); }
            } 

            proxy.Close();
        }
    }
}

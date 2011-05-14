using System;
using Agatha.Common;
using Agatha.Common.InversionOfControl;
using BonusBits.CodeSamples.Agatha.Messages;
using System.Threading;

namespace BonusBits.CodeSamples.Agatha.Client
{
    internal sealed class Program
    {
        private static void Main()
        {
            Console.WriteLine("Press any key enter when the service is ready.");
            Console.ReadKey(false);

            ComponentRegistration.Register();
            Console.WriteLine("To test sync press S, to test async press A.");
            
            ConsoleKeyInfo cki = Console.ReadKey(true);
            if (cki.Key == ConsoleKey.S)
            {
                for (Int32 i = 0; i < 6000; i++)
                {
                    CallTheService(i);
                }
            }
            else if (cki.Key == ConsoleKey.A)
            {
                for (Int32 i = 0; i < 6000; i++)
                {
                    CallTheServiceAsynchronously(i);
                }
            }

            Console.WriteLine("To exit press any key.");
            Console.ReadKey(false);
        }

        public static void CallTheService(Int32 i)
        {
            String userName = i.ToString();
            String password = userName;

            var requestDispatcher = IoC.Container.Resolve<IRequestDispatcher>();
            var response = requestDispatcher.Get<TestResponse>(
                new TestRequest() { UserName = userName, Password = password});

            Console.WriteLine(response.UserId);
        }

        public static void CallTheServiceAsynchronously(Int32 i)
        {
            String userName = i.ToString();
            String password = userName;

            var requestDispatcher = IoC.Container.Resolve<IAsyncRequestDispatcher>();
            requestDispatcher.Add(
                new TestRequest() { UserName = userName, Password = password });
            requestDispatcher.ProcessRequests(
                r => { Console.WriteLine(r.Get<TestResponse>().UserId); }, // Response.
                e => Console.WriteLine(e.ToString()));                     // Exception.
        }
    }
}

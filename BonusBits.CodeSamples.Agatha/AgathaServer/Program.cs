using System;
using System.Reflection;
using System.Runtime;
using System.ServiceModel;
using Agatha.ServiceLayer.WCF;
using BonusBits.CodeSamples.Agatha.Backend.Composition;

namespace BonusBits.CodeSamples.Agatha.Backend
{
    internal sealed class Program
    {
        private static void Main()
        {
            try { SelectIoC(); }
            catch(InvalidOperationException e)
            {
                Console.Write(e.Message);
                return;
            }

            Uri baseAddress = new Uri("http://localhost:8080/bonusbits-backend-agatha/");
            ServiceHost host = new ServiceHost(typeof(WcfRequestProcessor), baseAddress);
            host.Open();

            Console.Clear();
            Console.WriteLine("{0} is {1}running with server GC.",
                Assembly.GetEntryAssembly().GetName().Name,
                GCSettings.IsServerGC == true ? String.Empty : "not ");

            System.Console.ReadKey(false);
            host.Close();
        }

        private static void SelectIoC()
        {
            Console.WriteLine("Select IoC/DI container:");
            Console.WriteLine("Press C for Castle Windsor");
            Console.WriteLine("Press S for StructureMap");

            ConsoleKeyInfo cki = Console.ReadKey(false);

            switch (cki.Key)
            {
                case ConsoleKey.C:
                    CastleRegistration.Register();
                    break;

                case ConsoleKey.S:
                    StructureMapRegistration.Register();
                    break;
                default:
                    throw new InvalidOperationException("Please press either C or S keys.");
            }
        }
    }
}

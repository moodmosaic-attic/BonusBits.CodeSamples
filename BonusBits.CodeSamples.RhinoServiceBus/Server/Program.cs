using System;
using System.Reflection;
using System.Runtime;
using Rhino.ServiceBus.Hosting;

namespace BonusBits.CodeSamples.Rhino.ServiceBus.Backend
{
    internal sealed class Program
    {
        private static void Main()
        {
            String appConfig = Assembly.GetEntryAssembly().Location + ".config";
            var bootstrapper = new Bootstrapper(appConfig);

            var host = new RemoteAppDomainHost(bootstrapper.GetType());
            host.Start();

            Microsoft.Isam.Esent.Interop.SystemParameters.CacheSizeMax = 512;

            Console.WriteLine("{0} is {1}running with server GC.",
                Assembly.GetEntryAssembly().GetName().Name,
                GCSettings.IsServerGC == true ? String.Empty : "not ");

            Console.ReadKey(true);

            host.Close();
        }
    }
}
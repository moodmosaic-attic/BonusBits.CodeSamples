using System;
using System.Reflection;
using BonusBits.CodeSamples.Rhino.ServiceBus.Messages;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Rhino.ServiceBus;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;

internal sealed class Program
{
    private static void Main()
    {
        IWindsorContainer container = new WindsorContainer(new XmlInterpreter());
        container.Kernel.AddFacility("rhino.esb", new RhinoServiceBusFacility());
        container.Register(
            AllTypes.FromAssembly(Assembly.GetExecutingAssembly())
                    .Where(x => typeof(IMessageConsumer).IsAssignableFrom(x))
                    .Configure(registration => registration.LifeStyle.Is(LifestyleType.Transient)));

        var bus = container.Resolve<IStartableServiceBus>();
        bus.Start();

        SendMessages(bus, 6000);

        Console.ReadLine();
    }

    private static void SendMessages(IStartableServiceBus bus, Int32 count)
    {
        Console.WriteLine("Sending {0} messages", count);

        for (Int32 n = 0; n < count; n++)
        {
            bus.Send(new TestRequest() {
                UserName = "Nikos",
                Password = "Baxevanis"
            });
        }
     
        Console.WriteLine("Completed.");
    }
}

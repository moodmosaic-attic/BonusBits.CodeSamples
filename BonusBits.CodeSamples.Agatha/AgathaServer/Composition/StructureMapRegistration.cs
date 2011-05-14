using System.Reflection;
using Agatha.ServiceLayer;
using BonusBits.CodeSamples.Agatha.Backend.Domain;

namespace BonusBits.CodeSamples.Agatha.Backend.Composition
{
    internal static class StructureMapRegistration
    {
        private static readonly StructureMap.Container s_container;

        static StructureMapRegistration()
        {
            s_container = new StructureMap.Container();
        }

        public static void Register()
        {
            s_container.Configure(
                x => x.For<IWebService>()
                      .Use(c => new WebService(
                          c.GetInstance<IAuthenticator>(),
                          c.GetInstance<IStockQuote>())));

            s_container.Configure(
                x => x.For<IAuthenticator>()
                      .Use(c => new Authenticator(
                          c.GetInstance<ILogger>(),
                          c.GetInstance<IErrorHandler>(),
                          c.GetInstance<IDatabase>())));

            s_container.Configure(
                x => x.For<IStockQuote>()
                      .Use(c => new StockQuote(
                          c.GetInstance<ILogger>(),
                          c.GetInstance<IErrorHandler>(),
                          c.GetInstance<IDatabase>())));

            s_container.Configure(
                x => x.For<IDatabase>()
                      .Use(c => new Database(
                          c.GetInstance<ILogger>(),
                          c.GetInstance<IErrorHandler>())));

            s_container.Configure(
                x => x.For<IErrorHandler>()
                      .Use(c => new ErrorHandler(c.GetInstance<ILogger>())));

            s_container.Configure(
                 x => x.For<ILogger>()
                       .Use(new Logger()));

            Assembly messages = Assembly.Load("BonusBits.CodeSamples.Agatha.Messages");
            new ServiceLayerConfiguration(Assembly.GetExecutingAssembly(),
                messages, new global::Agatha.StructureMap.Container(s_container)).Initialize();
        }
    }
}

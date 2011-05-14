using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using NHibernate;
using NHibernate.ByteCode.Castle;
using NHibernate.Cfg;
using NHibernate.Cfg.Loquacious;
using NHibernate.Connection;
using NHibernate.Context;
using NHibernate.Dialect;
using NHibernate.Driver;
using WcfContracts;

namespace WcfServer
{
    internal static class Bootstrapper
    {
        public static void Initialize()
        {
            if (SessionFactoryHolder.DefaultSessionFactory != null) { return; }

            var cfg = new Configuration();
            cfg.CurrentSessionContext<WcfOperationSessionContext>()
               .DataBaseIntegration(ForSQLiteInMemory)
               .Proxy(p => p.ProxyFactoryFactory<ProxyFactoryFactory>())
               .SessionFactory()
                    .GenerateStatistics();

            SessionFactoryHolder.DefaultSessionFactory = cfg.BuildSessionFactory();
        }

        private static void ForSQLiteInMemory(IDbIntegrationConfigurationProperties db)
        {
            db.ConnectionString = "data source=:memory:";
            db.ConnectionReleaseMode = ConnectionReleaseMode.OnClose;
            db.Dialect<SQLiteDialect>();
            db.ConnectionProvider<DriverConnectionProvider>();
            db.Driver<SQLite20Driver>();
            db.BatchSize = 100;
        }
    }

    #region WcfOperationSessionContext Configuration and Wiring

    internal struct SessionFactoryHolder
    {
        public static ISessionFactory DefaultSessionFactory { get; set; }
    }

#if AGATHA
    [NHibernateWcfContext]
    public sealed class NHibernateWcfRequestProcessor : WcfRequestProcessor { }
#endif

    internal sealed class NHibernateWcfContextAttribute : Attribute, IServiceBehavior
    {
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, 
            Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters) { }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher channelDispatcher in serviceHostBase.ChannelDispatchers)
            {
                foreach (var endpoint in channelDispatcher.Endpoints)
                {
                    endpoint.DispatchRuntime.MessageInspectors.Add(new NHibernateWcfContextInitializer());
                }
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) { }
    }

    internal sealed class NHibernateWcfContextInitializer : IDispatchMessageInspector
    {
        public Object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            CurrentSessionContext.Bind(SessionFactoryHolder.DefaultSessionFactory.OpenSession());
            return null;
        }

        public void BeforeSendReply(ref Message reply, Object correlationState)
        {
            NHibernate.ISession session = CurrentSessionContext.Unbind(SessionFactoryHolder.DefaultSessionFactory);
            session.Dispose();
        }
    } 

    #endregion
}

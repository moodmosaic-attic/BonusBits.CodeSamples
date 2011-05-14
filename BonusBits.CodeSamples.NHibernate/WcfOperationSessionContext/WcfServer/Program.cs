using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using NHibernate;
using WcfContracts;

namespace WcfServer
{
    internal sealed class Program
    {
        public static void Main()
        {
            ServiceHost svh = new ServiceHost(typeof(WcfOperationSessionContextTestService));
            svh.AddServiceEndpoint(typeof(ICurrentSessionContextTestService),
                new NetTcpBinding(), "net.tcp://localhost:56789");
            svh.Open();

            Bootstrapper.Initialize();

            Console.WriteLine("Server ready. Press any key to exit..");
            Console.ReadKey(false);
            svh.Close();
        }
    }

    #region WcfOperationSessionContextTestService

    [NHibernateWcfContext]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    internal sealed class WcfOperationSessionContextTestService : ICurrentSessionContextTestService
    {
        private static readonly ISessionFactory s_sessionFactory = SessionFactoryHolder.DefaultSessionFactory;

        public void RunTests()
        {
            // Setup
            Console.WriteLine("\n[Testing WcfOperationSessionContext]");
            Console.WriteLine("SessionId={0}", s_sessionFactory.GetCurrentSession().GetSessionImplementation().SessionId);

            // Test 1
            Console.WriteLine("\nCalling GetCurrentSession multiple times will return the same session..");
            Debug.Assert(Object.ReferenceEquals(s_sessionFactory.GetCurrentSession(), s_sessionFactory.GetCurrentSession()) == true);
            Console.WriteLine("Passed..");

            // Test 2
            Console.WriteLine("\nCalling GetCurrentSession from different thread has no effect..");
            ManualResetEventSlim done = new ManualResetEventSlim(false);
            ThreadPool.QueueUserWorkItem((state) =>
            {
                try { ((ISessionFactory)state).GetCurrentSession(); }
                catch(NullReferenceException) { Console.WriteLine("Passed.."); }
                done.Set();
            }, s_sessionFactory);
            done.Wait();
        }
    } 

    #endregion
}

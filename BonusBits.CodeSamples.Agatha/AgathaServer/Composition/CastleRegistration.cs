using System;
using System.Linq;
using System.Reflection;
using Agatha.ServiceLayer;
using BonusBits.CodeSamples.Agatha.Backend.Domain;
using Castle.Core;
using Castle.DynamicProxy;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace BonusBits.CodeSamples.Agatha.Backend.Composition
{
    public static class CastleRegistration
    {
        private static readonly IWindsorContainer s_container;

        static CastleRegistration()
        {
            s_container = new WindsorContainer();
        }

        public static void Register()
        {
            RegisterTransient<IWebService, WebService>();
            RegisterTransient<IAuthenticator, Authenticator>();
            RegisterTransient<IStockQuote, StockQuote>();
            RegisterTransient<IDatabase, Database>();
            RegisterTransient<IErrorHandler, ErrorHandler>();

            RegisterTransient<ILogger, Logger>();

            Assembly messages = Assembly.Load("BonusBits.CodeSamples.Agatha.Messages");
            new ServiceLayerConfiguration(Assembly.GetExecutingAssembly(),
                messages, new global::Agatha.Castle.Container(s_container)).Initialize();
        }

        private static void RegisterTransient<T, R>()
        {
            s_container.Register(Component
                       .For(typeof(T))
                       .ImplementedBy(typeof(R))
                       .Named((typeof(T).Name))
                       .LifeStyle.Is(LifestyleType.Transient));
                       //.Interceptors<ReleaseComponentInterceptor>());
        }

        private static void RegisterSingleton<T, R>()
        {
            s_container.Register(Component
                       .For(typeof(T))
                       .ImplementedBy(typeof(R))
                       .Named((typeof(T).Name))
                       .LifeStyle.Is(LifestyleType.Singleton));
        }

        #region Nested Type: ReleaseComponentInterceptor
        /// <summary>
        /// http://kozmic.pl/archive/0001/01/01/transparently-releasing-components-in-windsor.aspx
        /// </summary>
        [Transient]
        private sealed class ReleaseComponentInterceptor : IInterceptor
        {
            private static readonly MethodInfo s_dispose;

            private readonly IKernel m_kernel;

            /// <summary>
            /// Initializes the <see cref="ReleaseComponentInterceptor"/> class.
            /// </summary>
            static ReleaseComponentInterceptor()
            {
                s_dispose = typeof(IDisposable).GetMethods().Single();
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ReleaseComponentInterceptor"/> class.
            /// </summary>
            /// <param name="kernel">The kernel.</param>
            public ReleaseComponentInterceptor(IKernel kernel)
            {
                m_kernel = kernel;
            }

            /// <summary>
            /// Intercepts the specified invocation.
            /// </summary>
            /// <param name="invocation">The invocation.</param>
            public void Intercept(IInvocation invocation)
            {
                if (invocation.Method == s_dispose)
                {
                    m_kernel.ReleaseComponent(invocation.Proxy);
                }
                else
                {
                    invocation.Proceed();
                }
            }
        } 
        #endregion
    }
}

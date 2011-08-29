using System;
using System.Web.Mvc;
using System.Web.Security;
using ExtensibleMvcApplication.Services;
using ExtensibleMvcApplication.Services.Implementation;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel.Unity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.StaticFactory;

namespace ExtensibleMvcApplication.Infrastructure.Unity
{
    internal static class Registrator
    {
        public static UnityContainer Install(this UnityContainer container, params Action<UnityContainer>[] installers)
        {
            foreach (var installer in installers)
            {
                installer.Invoke(container);
            }

            return container;
        }

        public static void ForControllers(UnityContainer container)
        {
            foreach (var type in typeof(Controllers.HomeController).Assembly.GetExportedTypes())
            {
                if (typeof(IController).IsAssignableFrom(type))
                {
                    container.RegisterType(
                        typeof(IController),
                        type,
                        type.FullName);
                }
            }
        }

        public static void ForServices(UnityContainer container)
        {
            container.RegisterType<MembershipProvider>(
                new InjectionFactory(x => Membership.Provider));

            container.RegisterType<IFormsAuthenticationService, FormsAuthenticationService>();
            container.RegisterType<IMembershipService, AccountMembershipService>();
        }

        // http://www.nikosbaxevanis.com/bonus-bits/2011/08/following-the-composition-root-pattern-with-enterprise-library.html
        public static void ForEnterpriseLibrary(UnityContainer container)
        {
            var configurator = new UnityContainerConfigurator(container);
            var configSource = ConfigurationSourceFactory.Create();

            EnterpriseLibraryContainer.ConfigureContainer(configurator, configSource);
        }
    }
}
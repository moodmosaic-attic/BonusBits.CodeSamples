using System;
using System.Collections.Generic;
using BonusBits.CodeSamples.WP7.Domain.Evans.Cargo;
using BonusBits.CodeSamples.WP7.Infrastructure.Composition;
using BonusBits.CodeSamples.WP7.Infrastructure.Repositories;
using Caliburn.Micro;
using Microsoft.Phone.Tasks;

namespace BonusBits.CodeSamples.WP7
{
    public sealed class AppBootstrapper : PhoneBootstrapper
    {
        private PhoneContainer m_container;

        /// <summary>
        /// Override to configure the framework and setup your IoC container.
        /// </summary>
        protected override void Configure()
        {
            m_container = new PhoneContainer();

            m_container.RegisterSingleton(
                typeof(MainPageViewModel),
                "MainPageViewModel",
                typeof(MainPageViewModel));

            m_container.RegisterSingleton(
               typeof(ThreadingPageViewModel),
               "ThreadingPageViewModel",
               typeof(ThreadingPageViewModel));

            m_container.RegisterSingleton(
               typeof(SterlingPageViewModel),
               "SterlingPageViewModel",
               typeof(SterlingPageViewModel));

            m_container.RegisterSingleton(
              typeof(SterlingExtensionsPageViewModel),
              "SterlingExtensionsPageViewModel",
              typeof(SterlingExtensionsPageViewModel));

            m_container.RegisterSingleton(
             typeof(CodeOnlyPageViewModel),
             "CodeOnlyPageViewModel",
             typeof(CodeOnlyPageViewModel));

            m_container.RegisterInstance(
                typeof(ICargoRepository),
                null,
                new CargoRepository());

            m_container.RegisterInstance(
                typeof(INavigationService),
                null,
                new FrameAdapter(RootFrame));

            m_container.RegisterInstance(
                typeof(IPhoneService),
                null,
                new PhoneApplicationServiceAdapter(PhoneService));

            m_container.Activator.InstallChooser<PhoneNumberChooserTask, PhoneNumberResult>();
            m_container.Activator.InstallLauncher<EmailComposeTask>();
        }

        /// <summary>
        /// Override this to provide an IoC specific implementation.
        /// </summary>
        /// <param name="service">The service to locate.</param>
        /// <param name="key">The key to locate.</param>
        /// <returns>The located service.</returns>
        protected override Object GetInstance(Type service, String key)
        {
            return m_container.GetInstance(service, key);
        }

        /// <summary>
        /// Override this to provide an IoC specific implementation
        /// </summary>
        /// <param name="service">The service to locate.</param>
        /// <returns>The located services.</returns>
        protected override IEnumerable<Object> GetAllInstances(Type service)
        {
            return m_container.GetAllInstances(service);
        }

        /// <summary>
        /// Override this to provide an IoC specific implementation.
        /// </summary>
        /// <param name="instance">The instance to perform injection on.</param>
        protected override void BuildUp(Object instance)
        {
            m_container.BuildUp(instance);
        }
    }
}

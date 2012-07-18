using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using CommonServiceLocator.WindsorAdapter;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Xunit;

namespace EntLibContrib.Common.Configuration.ContainerModel.Windsor.Tests
{
    public sealed class IntegrationTests
    {
        public IntegrationTests()
        {
            var container = new WindsorContainer();

            // Add a SubResolver for components with IEnumerable(Of T) dependencies on .ctors.
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));

            // This is the Windsor specific impl. of IContainerConfigurator interface.
            var configurator = new WindsorContainerConfigurator(container);

            // Configure the Enterprise Library Container to use Windsor internally.
            EnterpriseLibraryContainer.ConfigureContainer(configurator, 
                ConfigurationSourceFactory.Create());

            // Set the Current property to a new instance of the WindsorServiceLocator adapter.
            EnterpriseLibraryContainer.Current = new WindsorServiceLocator(container);
        }

        [Fact]
        public void Should_be_able_to_get_instance_for_service()
        {
            Assert.NotNull(EnterpriseLibraryContainer.Current.GetInstance<ExceptionManager>());
        }

        [Fact]
        public void Should_not_load_any_unity_assemblies_in_current_appdomain()
        {
            EnterpriseLibraryContainer.Current.GetInstance<ExceptionManager>();

            var unityAssemblies = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   where assembly.FullName.Contains("Unity")
                                   select assembly);

            Assert.Equal(0, unityAssemblies.Count());
        }

        [Fact]
        public void Should_try_to_locate_the_default_container_if_current_property_is_not_set()
        {
            try
            {
                Assembly.Load("Microsoft.Practices.Unity, Version=2.0.414.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");

                throw new InvalidOperationException(
                    "Should never be here, try to remove the Unity assemblies and run the test again");
            }
            catch(FileNotFoundException)
            {
                // HACK: We just verified that the Microsoft.Practices.Unity assembly can not be loaded.
                //       The default is to try to locate the Unity container if we don't pass the WindsorServiceLocator.
                EnterpriseLibraryContainer.Current = null;

                Assert.Throws<FileNotFoundException>(() =>
                {
                    EnterpriseLibraryContainer.Current.GetInstance<ExceptionManager>();
                });
            }
        }
    }
}

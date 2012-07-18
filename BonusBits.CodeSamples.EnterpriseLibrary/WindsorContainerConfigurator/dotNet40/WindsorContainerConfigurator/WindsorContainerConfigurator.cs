using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel;

namespace EntLibContrib.Common.Configuration.ContainerModel.Windsor
{
    public sealed class WindsorContainerConfigurator : IContainerConfigurator
    {
        private readonly IWindsorContainer m_container;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindsorContainerConfigurator"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public WindsorContainerConfigurator(IWindsorContainer container)
        {
            m_container = container;
        }

        /// <summary>
        /// Consume the set of <see cref="T:Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel.TypeRegistration"/> objects and
        /// configure the associated container.
        /// </summary>
        /// <param name="configurationSource">Configuration source to read registrations from.</param>
        /// <param name="rootProvider"><see cref="T:Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel.ITypeRegistrationsProvider"/> that knows how to
        /// read the <paramref name="configurationSource"/> and return all relevant type registrations.</param>
        public void RegisterAll(IConfigurationSource configurationSource, ITypeRegistrationsProvider rootProvider)
        {
            foreach (TypeRegistration registrationEntry in rootProvider.GetRegistrations(configurationSource))
            {
                Register(registrationEntry);
            }
        }

        /// <summary>
        /// Registers the specified registration entry. It will add a component registration to Windsor
        /// container from an Enterprise Library TypeRegistration.
        /// </summary>
        /// <param name="registrationEntry">The registration entry.</param>
        private void Register(TypeRegistration registrationEntry)
        {
            // Get any dependencies (if any) for passing to Windsor registration.
            var dependencies = GetRegistrationDependencies(registrationEntry);

            // Create the actual registration from the entry and the dependencies.
            var registration = CreateComponent(registrationEntry, dependencies);

            try
            {
                m_container.Register(registration);
            }
            catch (ComponentRegistrationException)
            {
                // TODO: Find out why this happen (probably has to do with the registrationEntry values).
                //       We are trying to register a component which is already registered by 
                //       the System.Type.FullName of the Castle.MicroKernel.Registration.ComponentRegistration<TService>.Implementation
                // HACK: Try to remove the component form the container and then register again using the TypeRegistration.Name as key.
                if (m_container.Kernel.RemoveComponent(registration.Name))
                {
                    registration = CreateComponent(registrationEntry, dependencies);
                    m_container.Register(registration.Named(registrationEntry.Name));
                }
            }
        }

        /// <summary>
        /// Creates the component for passing to Windsor registration.
        /// </summary>
        /// <param name="registrationEntry">The registration entry.</param>
        /// <returns></returns>
        private static ComponentRegistration<Object> CreateComponent(TypeRegistration registrationEntry)
        {
            return CreateComponent(registrationEntry, null);
        }

        /// <summary>
        /// Creates the component for passing to Windsor registration.
        /// </summary>
        /// <param name="registrationEntry">The registration entry.</param>
        /// <param name="dependencies">The dependencies.</param>
        /// <returns></returns>
        private static ComponentRegistration<Object> CreateComponent(TypeRegistration registrationEntry, Property[] dependencies)
        {
            Type implementation = registrationEntry.ImplementationType;

            var lifestyle = ToLifestyle(registrationEntry.Lifetime);
            var component =
                Component.For(registrationEntry.ServiceType)
                         .ImplementedBy(implementation)
                         .LifeStyle.Is(lifestyle);

            if (dependencies != null)
            {
                component.DependsOn(dependencies);
            }

            return component;
        }

        /// <summary>
        /// Converts a TypeRegistrationLifetime to a LifestyleType.
        /// </summary>
        /// <param name="lifetime">The lifetime.</param>
        /// <returns></returns>
        private static LifestyleType ToLifestyle(TypeRegistrationLifetime lifetime)
        {
            if (lifetime == TypeRegistrationLifetime.Singleton)
            {
                return LifestyleType.Singleton;
            }
            else if (lifetime == TypeRegistrationLifetime.Transient)
            {
                return LifestyleType.Transient;
            }
            else
            {
                throw new ArgumentOutOfRangeException("entry.Lifetime",
                    "Only Transient and Singleton are supported.");
            }
        }

        /// <summary>
        /// Gets the registration dependencies (based on UnityContainerConfiguration implementation).
        /// </summary>
        /// <param name="registrationEntry">The registration entry.</param>
        /// <returns></returns>
        private static Property[] GetRegistrationDependencies(TypeRegistration registrationEntry)
        {
            List<Property[]> dependencyMembers =
                (from parameterValue in registrationEntry.ConstructorParameters
                 select GetInjectionParameterValue(parameterValue)).ToList();

            dependencyMembers.Add(
               (from injected in registrationEntry.InjectedProperties
                select Property.ForKey(injected.PropertyName)
                               .Eq(GetInjectionParameterValue(injected.PropertyValue)))
                               .ToArray());

            return dependencyMembers.SelectMany(x => x).ToArray();
        }

        /// <summary>
        /// Gets the injection parameter value.
        /// </summary>
        /// <param name="dependencyParameter">The dependency parameter.</param>
        /// <returns></returns>
        private static Property[] GetInjectionParameterValue(ParameterValue dependencyParameter)
        {
            var visitor = new WindsorParameterVisitor();
            visitor.Visit(dependencyParameter);
            return visitor.InjectionParameters;
        }

        private sealed class WindsorParameterVisitor : ParameterValueVisitor
        {
            /// <summary>
            /// Gets or sets the injection parameters.
            /// </summary>
            /// <value>
            /// The injection parameters.
            /// </value>
            public Property[] InjectionParameters { get; private set; }

            /// <summary>
            /// The method called when a <see cref="T:Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel.ConstantParameterValue"/> object is visited.
            /// </summary>
            /// <param name="parameterValue">The <see cref="T:Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel.ConstantParameterValue"/> to process.</param>
            protected override void VisitConstantParameterValue(ConstantParameterValue parameterValue)
            {
                String key = ((MemberExpression)parameterValue.Expression).Member.Name;
                InjectionParameters = new Property[] { Property.ForKey(key).Eq(parameterValue.Value) };
            }

            /// <summary>
            /// The method called when a <see cref="T:Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel.ContainerResolvedParameter"/> object is visited.
            /// </summary>
            /// <param name="parameterValue">The <see cref="T:Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel.ContainerResolvedParameter"/> to process.</param>
            protected override void VisitResolvedParameterValue(ContainerResolvedParameter parameterValue)
            {
                InjectionParameters = new Property[] { Property.ForKey(parameterValue.Type).Is(parameterValue.Name) };
            }

            /// <summary>
            /// The method called when a <see cref="T:Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel.ContainerResolvedEnumerableParameter"/> object is visited.
            /// </summary>
            /// <param name="parameterValue">The <see cref="T:Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel.ContainerResolvedEnumerableParameter"/> to process.</param>
            protected override void VisitEnumerableParameterValue(ContainerResolvedEnumerableParameter parameterValue)
            {
                InjectionParameters = parameterValue.Names
                        .Select(name => Property.ForKey(parameterValue.ElementType).Is(name))
                        .ToArray();
            }
        }
    }
}

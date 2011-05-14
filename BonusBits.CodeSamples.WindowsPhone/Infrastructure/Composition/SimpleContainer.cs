using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BonusBits.CodeSamples.WP7.Infrastructure.Composition
{
    internal abstract class SimpleContainer
    {
        private readonly IList<ContainerEntry> m_entries;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleContainer"/> class.
        /// </summary>
        protected SimpleContainer()
        {
            m_entries = new List<ContainerEntry>();
        }

        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="key">The key.</param>
        /// <param name="implementation">The implementation.</param>
        public void RegisterInstance(Type service, String key, Object implementation)
        {
            RegisterHandler(service, key, () => implementation);
        }

        /// <summary>
        /// Registers the per request.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="key">The key.</param>
        /// <param name="implementation">The implementation.</param>
        public void RegisterPerRequest(Type service, String key, Type implementation)
        {
            RegisterHandler(service, key, () => BuildInstance(implementation));
        }

        /// <summary>
        /// Registers the singleton.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="key">The key.</param>
        /// <param name="implementation">The implementation.</param>
        public void RegisterSingleton(Type service, String key, Type implementation)
        {
            Object singleton = null;
            RegisterHandler(service, key,
                () => singleton ?? (singleton = BuildInstance(implementation)));
        }

        /// <summary>
        /// Registers the handler.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="key">The key.</param>
        /// <param name="handler">The handler.</param>
        public void RegisterHandler(Type service, String key, Func<Object> handler)
        {
            GetOrCreateEntry(service, key).Add(handler);
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public Object GetInstance(Type service, String key)
        {
            ContainerEntry entry = GetEntry(service, key);
            if (entry != null)
            {
                return entry.Single()();
            }

            if (typeof(Delegate).IsAssignableFrom(service))
            {
                Type typeToCreate = service.GetGenericArguments()[0];
                Type factoryFactoryType = typeof(FactoryFactory<>).MakeGenericType(typeToCreate);
                Object factoryFactoryHost = Activator.CreateInstance(factoryFactoryType);
                MethodInfo factoryFactoryMethod = factoryFactoryType.GetMethod("Create");
                return factoryFactoryMethod.Invoke(factoryFactoryHost, new Object[] { this });
            }

            if (typeof(IEnumerable).IsAssignableFrom(service))
            {
                Type listType = service.GetGenericArguments()[0];
                IList<Object> instances = GetAllInstances(listType).ToList();
                Array array = Array.CreateInstance(listType, instances.Count);

                for (Int32 i = 0; i < array.Length; i++)
                {
                    array.SetValue(instances[i], i);
                }

                return array;
            }

            return null;
        }

        /// <summary>
        /// Gets all instances.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        public IEnumerable<Object> GetAllInstances(Type service)
        {
            ContainerEntry entry = GetEntry(service, null);
            return entry != null ? entry.Select(x => x()) : new Object[0];
        }

        /// <summary>
        /// Builds up.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public void BuildUp(Object instance)
        {
            IEnumerable<PropertyInfo> injectables = instance.GetType().GetProperties()
                .Where((property) =>
                {
                    return property.CanRead && property.CanWrite &&
                        property.PropertyType.IsInterface;
                });

            foreach (PropertyInfo injectee in injectables)
            {
                IEnumerable<Object> injection = GetAllInstances(injectee.PropertyType);
                if (injection.Any())
                {
                    injectee.SetValue(instance, injection.First(), null);
                }
            }
        }

        /// <summary>
        /// Builds the instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        protected Object BuildInstance(Type type)
        {
            Object[] args = DetermineConstructorArgs(type);
            return ActivateInstance(type, args);
        }

        /// <summary>
        /// Activates the instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        protected virtual Object ActivateInstance(Type type, Object[] args)
        {
            return args.Length > 0 ? Activator.CreateInstance(type, args) : Activator.CreateInstance(type);
        }

        #region Private Methods
        /// <summary>
        /// Gets the or create entry.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        private ContainerEntry GetOrCreateEntry(Type service, String key)
        {
            ContainerEntry entry = GetEntry(service, key);
            if (entry == null)
            {
                entry = new ContainerEntry { Service = service, Key = key };
                m_entries.Add(entry);
            }

            return entry;
        }

        /// <summary>
        /// Gets the entry.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        private ContainerEntry GetEntry(Type service, String key)
        {
            return service == null
                ? m_entries.Where(x => x.Key == key).FirstOrDefault()
                : m_entries.Where(x => x.Service == service && x.Key == key).FirstOrDefault();
        }

        /// <summary>
        /// Determines the constructor args.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <returns></returns>
        private Object[] DetermineConstructorArgs(Type implementation)
        {
            List<Object> args = new List<Object>();
            ConstructorInfo constructor = SelectEligibleConstructor(implementation);

            if (constructor != null)
            {
                args.AddRange(constructor.GetParameters()
                    .Select(info => GetInstance(info.ParameterType, null)));
            }

            return args.ToArray();
        }

        /// <summary>
        /// Selects the eligible constructor.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private static ConstructorInfo SelectEligibleConstructor(Type type)
        {
            return type.GetConstructors().OrderByDescending((c) => { return c.GetParameters().Length; })
                                         .FirstOrDefault<ConstructorInfo>();
        }
        #endregion

        #region Nested Type : ContainerEntry
        private sealed class ContainerEntry : List<Func<Object>>
        {
            public String Key;
            public Type Service;
        }
        #endregion

        #region Nested Type : FactoryFactory
        private sealed class FactoryFactory<T>
        {
            /// <summary>
            /// Creates the specified container.
            /// </summary>
            /// <param name="container">The container.</param>
            /// <returns></returns>
            public Func<T> Create(SimpleContainer container)
            {
                return () => (T)container.GetInstance(typeof(T), null);
            }
        }
        #endregion
    }
}

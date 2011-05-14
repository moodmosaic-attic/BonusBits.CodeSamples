using System;
using Caliburn.Micro;

namespace BonusBits.CodeSamples.WP7.Infrastructure.Composition
{
    internal sealed class PhoneContainer : SimpleContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PhoneContainer"/> class.
        /// </summary>
        public PhoneContainer()
        {
            Activator = new InstanceActivator(type => GetInstance(type, null));
        }

        /// <summary>
        /// Gets or sets the activator.
        /// </summary>
        /// <value>The activator.</value>
        public InstanceActivator Activator { get; private set; }

        /// <summary>
        /// Activates the instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        protected override Object ActivateInstance(Type type, Object[] args)
        {
            return Activator.ActivateInstance(base.ActivateInstance(type, args));
        }

        /// <summary>
        /// Registers the with phone service.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        public void RegisterWithPhoneService<TService, TImplementation>()
            where TImplementation : TService
        {
            RegisterHandler(typeof(TService), null, () =>
            {
                var phoneService = (IPhoneService)GetInstance(typeof(IPhoneService), null);

                if (phoneService.State.ContainsKey(typeof(TService).FullName))
                {
                    return phoneService.State[typeof(TService).FullName];
                }

                var instance = BuildInstance(typeof(TImplementation));

                phoneService.State[typeof(TService).FullName] = instance;

                return instance;
            });
        }
    }
}

using System;
using System.IO;
using Castle.Facilities.FactorySupport;
using Rhino.ServiceBus.Hosting;

namespace BonusBits.CodeSamples.Rhino.ServiceBus.Backend
{
    internal sealed class Bootstrapper : AbstractBootStrapper
    {
        private readonly FileInfo m_configFileInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bootstrapper"/> class.
        /// The default .ctor is required to start Rhino.ServiceBus.Hosting.RemoteAppDomainHost.
        /// </summary>
        public Bootstrapper() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bootstrapper"/> class.
        /// </summary>
        /// <param name="configFile">The config file.</param>
        public Bootstrapper(String configFile)
        {
            m_configFileInfo = new FileInfo(configFile);
            if (!m_configFileInfo.Exists)
            {
                throw new FileNotFoundException("configFile");
            }
        }

        /// <summary>
        /// Configures the container.
        /// </summary>
        protected override void ConfigureContainer()
        {
            container.Kernel.AddFacility("factory", new FactorySupportFacility());

            base.ConfigureContainer();
        }
    }
}

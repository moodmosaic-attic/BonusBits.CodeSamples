# Goals

Implementation of IContainerConfigurator interface (Enterprise Library 5.0) to create an object that 
can read a set of TypeRegistration objects representing the current Enterprise Library configuration
and configure the Castle Windsor dependency injection container with that information.

# What is implemented

- Service Types to Implementation Types registration
- Handling of Constructor Injection
- Handling of Property Injection

# Limitations

Registering some TypeRegistration entries results in a ComponentRegistrationException to be thrown by
Castle Windsor's MicroKernel. The work-around is to try to register by TypeRegistration.Name and not by
the System.Type.FullName of the Castle.MicroKernel.Registration.ComponentRegistration<TService>.Implementation.

# Usage

On application's composition root:

	var container = new WindsorContainer();

	// Add a SubResolver for components with IEnumerable(Of T) dependencies on .ctors.
	container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));

	// This is the Windsor specific impl. of IContainerConfigurator interface.
	var configurator = new WindsorContainerConfigurator(container);

	// Configure the Enterprise Library Container to use Windsor internally.
	EnterpriseLibraryContainer.ConfigureContainer(
		configurator, ConfigurationSourceFactory.Create());

	// Set the Current property to a new instance of the WindsorServiceLocator adapter.
	EnterpriseLibraryContainer.Current = new WindsorServiceLocator(container);

# Examples

You can resolve a component and the Enterprise Library will use Windsor as the underlying container:

	var exceptionManager = EnterpriseLibraryContainer.Current.GetInstance<ExceptionManager>();

# Future work

- Try to support extremely complex TypeRegistration scenarios.

# License 

Enterprise Library is provided under the terms of Microsoft Public License (Ms-PL), Castle Project is provided under the terms of Apache Software Foundation License 2.0.
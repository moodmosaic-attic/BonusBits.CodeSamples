using System;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.Unity;

namespace ExtensibleMvcApplication.Infrastructure.Unity
{
    internal sealed class UnityControllerFactory : DefaultControllerFactory
    {
        private readonly UnityContainer container;

        public UnityControllerFactory(UnityContainer container)
        {
            this.container = container;
        }

        /// <summary>
        /// Retrieves the controller instance for the specified request context and controller type.
        /// </summary>
        /// <param name="requestContext">The context of the HTTP request, which includes the HTTP context and route data.</param>
        /// <param name="controllerType">The type of the controller.</param>
        /// <returns>
        /// The controller instance.
        /// </returns>
        /// <exception cref="T:System.Web.HttpException">
        ///   <paramref name="controllerType"/> is null.</exception>
        ///   
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="controllerType"/> cannot be assigned.</exception>
        ///   
        /// <exception cref="T:System.InvalidOperationException">An instance of <paramref name="controllerType"/> cannot be created.</exception>
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            IController controller;

            if (controllerType == null)
            {
                return null;
            }

            if (!typeof(IController).IsAssignableFrom(controllerType))
            {
                throw new ArgumentException(string.Format("Type requested is not a controller: {0}", controllerType.Name), "controllerType");
            }

            try
            {
                controller = container.Resolve(controllerType) as IController;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(String.Format("Error resolving controller {0}", controllerType.Name), e);
            }

            return controller;
        }
    }
}
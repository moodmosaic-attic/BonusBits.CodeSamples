using System.Collections.Generic;
using BonusBits.CodeSamples.WP7.Domain.Evans.Cargo;

namespace BonusBits.CodeSamples.WP7.Domain.Evans
{
    /// <summary>
    /// Definition of a routing external service.
    /// http://dddsample.sourceforge.net/
    /// </summary>
    public interface IRoutingService
    {
        /// <summary>
        /// Finds all possible routes that satisfy a given specification.
        /// </summary>
        /// <param name="routeSpecification">Description of route.</param>
        /// <returns>A list of itineraries that satisfy the specification. 
        /// May be an empty list if no route is found.</returns>
        IList<Itinerary> FetchRoutesForSpecification(RouteSpecification routeSpecification);
    }
}

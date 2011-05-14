using System;
using Caliburn.Micro;

namespace BonusBits.CodeSamples.WP7.Domain.Evans.Cargo
{
    /// <summary>
    /// Raised after cargo has arrived assigned to route.
    /// http://dddsample.sourceforge.net/
    /// </summary>
    public sealed class CargoHasBeenAssignedToRouteEvent : IHandle<Cargo>
    {
        private readonly Itinerary m_oldItinerary;

        /// <summary>
        /// Initializes a new instance of the <see cref="CargoHasBeenAssignedToRouteEvent"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="oldItinerary">The old itinerary.</param>
        public CargoHasBeenAssignedToRouteEvent(Cargo source, Itinerary oldItinerary)
        {
            m_oldItinerary = oldItinerary;
        }

        /// <summary>
        /// Gets the route before assigning cargo to a new one.
        /// </summary>
        public Itinerary OldItinerary
        {
            get { return m_oldItinerary; }
        }

        #region IHandle<Cargo> Members

        /// <summary>
        /// Handles the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(Cargo message)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

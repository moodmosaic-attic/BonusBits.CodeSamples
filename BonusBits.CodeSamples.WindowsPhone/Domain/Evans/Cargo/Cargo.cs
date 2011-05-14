using System;
using BonusBits.CodeSamples.WP7.Domain.Evans.Handling;
using Caliburn.Micro;

namespace BonusBits.CodeSamples.WP7.Domain.Evans.Cargo
{
    /// <summary>
    /// Cargo.
    /// http://dddsample.sourceforge.net/
    /// </summary>
    public class Cargo
    {
        private readonly IEventAggregator m_eventAggegator;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cargo"/> class.
        /// </summary>
        public Cargo() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cargo"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        public Cargo(IEventAggregator eventAggregator)
        {
            m_eventAggegator = eventAggregator;
        }

        /// <summary>
        /// Gets the tracking id of this cargo.
        /// </summary>
        public TrackingId TrackingId { get; protected set; }

        /// <summary>
        /// Gets the route specification of this cargo.
        /// </summary>
        public RouteSpecification RouteSpecification { get; protected set; }

        /// <summary>
        /// Gets the itinerary of this cargo.
        /// </summary>
        public Itinerary Itinerary { get; protected set; }

        /// <summary>
        /// Gets delivery status of this cargo.
        /// </summary>
        public Delivery Delivery { get; protected set; }

        /// <summary>
        /// Creates new <see cref="Cargo"/> object with provided tracking id and route specification.
        /// </summary>
        /// <param name="trackingId">Tracking id of this cargo.</param>
        /// <param name="routeSpecification">Route specification.</param>
        public Cargo(TrackingId trackingId, RouteSpecification routeSpecification)
        {
            if (trackingId == null)
            {
                throw new ArgumentNullException("trackingId");
            }

            if (routeSpecification == null)
            {
                throw new ArgumentNullException("routeSpecification");
            }

            TrackingId         = trackingId;
            RouteSpecification = routeSpecification;
            Delivery           = Delivery.DerivedFrom(RouteSpecification, Itinerary, null);
        }

        /// <summary>
        /// Specifies a new route for this cargo.
        /// </summary>
        /// <param name="routeSpecification">Route specification.</param>
        public void SpecifyNewRoute(RouteSpecification routeSpecification)
        {
            if (routeSpecification == null)
            {
                throw new ArgumentNullException("routeSpecification");
            }

            RouteSpecification = routeSpecification;
            Delivery           = Delivery.UpdateOnRouting(RouteSpecification, Itinerary);
        }

        /// <summary>
        /// Assigns cargo to a provided route.
        /// </summary>
        /// <param name="itinerary">New itinerary</param>
        public void AssignToRoute(Itinerary itinerary)
        {
            if (itinerary == null)
            {
                throw new ArgumentNullException("itinerary");
            }

            var @event = new CargoHasBeenAssignedToRouteEvent(this, Itinerary);
            Itinerary  = itinerary;
            Delivery   = Delivery.UpdateOnRouting(RouteSpecification, Itinerary);

            m_eventAggegator.Publish<CargoHasBeenAssignedToRouteEvent>(@event);
        }

        /// <summary>
        /// Updates delivery progress information according to handling history.
        /// </summary>
        /// <param name="lastHandlingEvent">Most recent handling event.</param>
        public void DeriveDeliveryProgress(HandlingEvent lastHandlingEvent)
        {
            Delivery = Delivery.DerivedFrom(RouteSpecification, Itinerary, lastHandlingEvent);

            if (Delivery.IsMisdirected)
            {
                m_eventAggegator.Publish<CargoWasMisdirectedEvent>(
                    new CargoWasMisdirectedEvent(this));
            }
            else if (Delivery.IsUnloadedAtDestination)
            {
                m_eventAggegator.Publish<CargoHasArrivedEvent>(
                    new CargoHasArrivedEvent(this));
            }
        }
    }
}

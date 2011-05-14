using System;
using System.Collections.Generic;
using System.Linq;
using BonusBits.CodeSamples.WP7.Domain.Evans.Handling;

namespace BonusBits.CodeSamples.WP7.Domain.Evans.Cargo
{
    /// <summary>
    /// Description of delivery status.
    /// http://dddsample.sourceforge.net/
    /// </summary>
#pragma warning disable 661,660 //Equals and GetHashCode are overridden in ValueObject class.
    public sealed class Delivery : ValueObject
#pragma warning restore 661,660
    {
        private readonly TransportStatus    m_transportStatus;
        private readonly Location.Location  m_lastKnownLocation;
        private readonly Boolean            m_misdirected;
        private readonly Nullable<DateTime> m_eta;
        private readonly Boolean            m_isUnloadedAtDestination;
        private readonly RoutingStatus      m_routingStatus;
        private readonly DateTime           m_calculatedAt;
        private readonly HandlingEvent      m_lastEvent;
        private readonly HandlingActivity   m_nextExpectedActivity;

        /// <summary>
        /// Initializes a new instance of the <see cref="Delivery"/> class.
        /// </summary>
        public Delivery() { }

        /// <summary>
        /// Gets next expected activity.
        /// </summary>
        public HandlingActivity NextExpectedActivity
        {
            get { return m_nextExpectedActivity; }
        }

        /// <summary>
        /// Gets status of cargo routing.
        /// </summary>
        public RoutingStatus RoutingStatus
        {
            get { return m_routingStatus; }
        }

        /// <summary>
        /// Gets time when this delivery status was calculated.
        /// </summary>
        public DateTime CalculatedAt
        {
            get { return m_calculatedAt; }
        }

        /// <summary>
        /// Gets if this cargo has been unloaded at its destination.
        /// </summary>
        public Boolean IsUnloadedAtDestination
        {
            get { return m_isUnloadedAtDestination; }
        }

        /// <summary>
        /// Gets estimated time of arrival. Returns null if information cannot be obtained (cargo is misrouted).
        /// </summary>
        public Nullable<DateTime> EstimatedTimeOfArrival
        {
            get { return m_eta; }
        }

        /// <summary>
        /// Gets last known location of this cargo.
        /// </summary>
        public Location.Location LastKnownLocation
        {
            get { return m_lastKnownLocation; }
        }

        /// <summary>
        /// Gets status of cargo transport.
        /// </summary>
        public TransportStatus TransportStatus
        {
            get { return m_transportStatus; }
        }

        /// <summary>
        /// Gets if this cargo was misdirected.
        /// </summary>
        public Boolean IsMisdirected
        {
            get { return m_misdirected; }
        }

        /// <summary>
        /// Creates a new delivery snapshot based on the complete handling history of a cargo, as well 
        /// as its route specification and itinerary.
        /// </summary>
        /// <param name="specification">Current route specification.</param>
        /// <param name="itinerary">Current itinerary.</param>
        /// <param name="lastHandlingEvent">Most recent handling event.</param>
        /// <returns>Delivery status description.</returns>
        public static Delivery DerivedFrom(
            RouteSpecification specification,
            Itinerary itinerary,
            HandlingEvent lastHandlingEvent)
        {
            return new Delivery(lastHandlingEvent, itinerary, specification);
        }

        /// <summary>
        /// Creates a new delivery snapshot to reflect changes in routing, i.e. when the route 
        /// specification or the itinerary has changed but no additional handling of the 
        /// cargo has been performed.
        /// </summary>
        /// <param name="routeSpecification">Current route specification.</param>
        /// <param name="itinerary">Current itinerary.</param>
        /// <returns>New delivery status description.</returns>
        public Delivery UpdateOnRouting(RouteSpecification routeSpecification, Itinerary itinerary)
        {
            if (routeSpecification == null)
            {
                throw new ArgumentNullException("routeSpecification");
            }
            return new Delivery(m_lastEvent, itinerary, routeSpecification);
        }

        private Delivery(HandlingEvent lastHandlingEvent, Itinerary itinerary,
            RouteSpecification specification)
        {
            m_calculatedAt = DateTime.Now;
            m_lastEvent = lastHandlingEvent;

            m_misdirected = CalculateMisdirectionStatus(itinerary);
            m_routingStatus = CalculateRoutingStatus(itinerary, specification);
            m_transportStatus = CalculateTransportStatus();
            m_lastKnownLocation = CalculateLastKnownLocation();
            m_eta = CalculateEta(itinerary);
            m_nextExpectedActivity = CalculateNextExpectedActivity(specification, itinerary);
            m_isUnloadedAtDestination = CalculateUnloadedAtDestination(specification);
        }

        private Boolean CalculateUnloadedAtDestination(RouteSpecification specification)
        {
            return LastEvent != null &&
                     LastEvent.EventType == HandlingEventType.Unload &&
                     specification.Destination == LastEvent.Location;
        }

        private Nullable<DateTime> CalculateEta(Itinerary itinerary)
        {
            return OnTrack ? itinerary.FinalArrivalDate : null;
        }

        private Location.Location CalculateLastKnownLocation()
        {
            return LastEvent != null ? LastEvent.Location : null;
        }

        private TransportStatus CalculateTransportStatus()
        {
            if (LastEvent == null)
            {
                return TransportStatus.NotReceived;
            }

            switch (LastEvent.EventType)
            {
                case HandlingEventType.Load:
                    return TransportStatus.OnboardCarrier;

                case HandlingEventType.Unload:
                case HandlingEventType.Receive:
                case HandlingEventType.Customs:
                    return TransportStatus.InPort;

                case HandlingEventType.Claim:
                    return TransportStatus.Claimed;

                default:
                    return TransportStatus.Unknown;
            }
        }

        private HandlingActivity CalculateNextExpectedActivity(RouteSpecification routeSpecification, Itinerary itinerary)
        {
            if (!OnTrack)
            {
                return null;
            }

            if (LastEvent == null)
            {
                return new HandlingActivity(HandlingEventType.Receive, routeSpecification.Origin);
            }

            switch (LastEvent.EventType)
            {
                case HandlingEventType.Load:
                    Leg lastLeg = itinerary.Legs.FirstOrDefault(x => x.LoadLocation == LastEvent.Location);
                    return lastLeg != null ? new HandlingActivity(HandlingEventType.Unload, lastLeg.UnloadLocation) : null;

                case HandlingEventType.Unload:
                    IEnumerator<Leg> enumerator = itinerary.Legs.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.UnloadLocation == LastEvent.Location)
                        {
                            Leg currentLeg = enumerator.Current;
                            return enumerator.MoveNext()
                                ? new HandlingActivity(HandlingEventType.Load, enumerator.Current.LoadLocation)
                                : new HandlingActivity(HandlingEventType.Claim, currentLeg.UnloadLocation);
                        }
                    }
                    return null;

                case HandlingEventType.Receive:
                    Leg firstLeg = itinerary.Legs.First();
                    return new HandlingActivity(HandlingEventType.Load, firstLeg.LoadLocation);

                default:
                    return null;
            }
        }

        private static RoutingStatus CalculateRoutingStatus(Itinerary itinerary, RouteSpecification specification)
        {
            if (itinerary == null)
            {
                return RoutingStatus.NotRouted;
            }
            return specification.IsSatisfiedBy(itinerary) ? RoutingStatus.Routed : RoutingStatus.Misrouted;
        }

        private Boolean CalculateMisdirectionStatus(Itinerary itinerary)
        {
            if (LastEvent == null)
            {
                return false;
            }
            return !itinerary.IsExpected(LastEvent);
        }

        private Boolean OnTrack
        {
            get { return RoutingStatus == RoutingStatus.Routed && !IsMisdirected; }
        }

        private HandlingEvent LastEvent
        {
            get { return m_lastEvent; }
        }

        #region Infrastructure

        protected override IEnumerable<Object> GetAtomicValues()
        {
            yield return m_calculatedAt;
            yield return m_eta;
            yield return m_lastEvent;
            yield return m_isUnloadedAtDestination;
            yield return m_isUnloadedAtDestination;
            yield return m_lastKnownLocation;
            yield return m_misdirected;
            yield return m_routingStatus;
            yield return m_transportStatus;
        }

        public static Boolean operator ==(Delivery left, Delivery right)
        {
            return EqualOperator(left, right);
        }

        public static Boolean operator !=(Delivery left, Delivery right)
        {
            return NotEqualOperator(left, right);
        }
        #endregion
    }
}

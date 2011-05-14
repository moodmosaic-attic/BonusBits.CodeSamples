using System;
using System.Collections.Generic;

namespace BonusBits.CodeSamples.WP7.Domain.Evans.Cargo
{
    /// <summary>
    /// Contains information about a route: its origin, destination and arrival deadline.
    /// http://dddsample.sourceforge.net/
    /// </summary>
#pragma warning disable 661,660 //Equals and GetHashCode are overridden in ValueObject class.
    public sealed class RouteSpecification : ValueObject
#pragma warning restore 661,660
    {
        private readonly Location.Location m_origin;
        private readonly Location.Location m_destination;

        private readonly DateTime m_arrivalDeadline;

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteSpecification"/> class.
        /// </summary>
        public RouteSpecification() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteSpecification"/> class.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="arrivalDeadline">The arrival deadline.</param>
        public RouteSpecification(
            Location.Location origin,
            Location.Location destination,
            DateTime arrivalDeadline)
        {
            if (origin == null)
            {
                throw new ArgumentNullException("origin");
            }

            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }

            if (origin == destination)
            {
                throw new ArgumentException("Origin and destination can't be the same.");
            }

            m_origin          = origin;
            m_arrivalDeadline = arrivalDeadline;
            m_destination     = destination;
        }

        /// <summary>
        /// Checks whether provided itinerary (a description of transporting steps) satisfies this
        /// specification.
        /// </summary>
        /// <param name="itinerary">An itinerary.</param>
        /// <returns>True, if cargo can be transported from <see cref="Origin"/> to <see cref="Destination"/>
        /// before <see cref="ArrivalDeadline"/> using provided itinerary.
        /// </returns>
        public Boolean IsSatisfiedBy(Itinerary itinerary)
        {
            return Origin == itinerary.InitialDepartureLocation &&
                   Destination == itinerary.FinalArrivalLocation &&
                   ArrivalDeadline > itinerary.FinalArrivalDate;
        }

        /// <summary>
        /// Date of expected cargo arrival.
        /// </summary>
        public DateTime ArrivalDeadline
        {
            get { return m_arrivalDeadline; }
        }

        /// <summary>
        /// Location where cargo should be delivered.
        /// </summary>
        public Location.Location Destination
        {
            get { return m_destination; }
        }

        /// <summary>
        /// Location where cargo should be picked up.
        /// </summary>
        public Location.Location Origin
        {
            get { return m_origin; }
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Boolean operator ==(RouteSpecification left, RouteSpecification right)
        {
            return EqualOperator(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Boolean operator !=(RouteSpecification left, RouteSpecification right)
        {
            return NotEqualOperator(left, right);
        }

        /// <summary>
        /// To be overridden in inheriting clesses for providing a collection of atomic values of
        /// this Value Object.
        /// </summary>
        /// <returns>Collection of atomic values.</returns>
        protected override IEnumerable<Object> GetAtomicValues()
        {
            yield return m_origin;
            yield return m_destination;
            yield return m_arrivalDeadline;
        }
    }
}

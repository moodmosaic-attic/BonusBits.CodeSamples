using System;
using System.Collections.Generic;
using System.Linq;
using BonusBits.CodeSamples.WP7.Domain.Evans.Handling;

namespace BonusBits.CodeSamples.WP7.Domain.Evans.Cargo
{
    /// <summary>
    /// Specifies steps required to transport a cargo from its origin to destination.
    /// http://dddsample.sourceforge.net/
    /// </summary>
#pragma warning disable 661,660 //Equals and GetHashCode are overridden in ValueObject class.
    public sealed class Itinerary : ValueObject
#pragma warning restore 661,660
    {
        private readonly IList<Leg> m_legs;

        /// <summary>
        /// Initializes a new instance of the <see cref="Itinerary"/> class.
        /// </summary>
        public Itinerary() { }

        /// <summary>
        /// Creates new <see cref="Itinerary"/> instance for provided collection of routing steps (legs).
        /// </summary>
        /// <param name="legs">Collection of routing steps (legs).</param>
        public Itinerary(IEnumerable<Leg> legs)
        {
            m_legs = new List<Leg>(legs);
        }

        /// <summary>
        /// Gets unmodifiable collection of this itinerary's legs.
        /// </summary>
        public IEnumerable<Leg> Legs
        {
            get { return m_legs; }
        }

        /// <summary>
        /// Gets the location of first departure according to this itinerary.
        /// </summary>
        public Location.Location InitialDepartureLocation
        {
            get { return IsEmpty ? Location.Location.Unknown : m_legs.First().LoadLocation; }
        }

        /// <summary>
        /// Gets the location of last arrival according to this itinerary.
        /// </summary>
        public Location.Location FinalArrivalLocation
        {
            get { return IsEmpty ? Location.Location.Unknown : m_legs.Last().UnloadLocation; }
        }

        /// <summary>
        /// Gets the time of last arrival according to this itinerary. 
        /// Returns null for empty itinerary.
        /// </summary>
        public Nullable<DateTime> FinalArrivalDate
        {
            get { return IsEmpty ? (Nullable<DateTime>)null : m_legs.Last().UnloadDate; }
        }

        /// <summary>
        /// Checks whether provided event is expected according to this itinerary specification.
        /// </summary>
        /// <param name="event">A handling event.</param>
        /// <returns>True, if it is expected. Otherwise - false. 
        /// If itinerary is empty, returns false.</returns>
        public Boolean IsExpected(HandlingEvent @event)
        {
            if (IsEmpty)
            {
                return false;
            }

            if (@event.EventType == HandlingEventType.Receive)
            {
                Leg firstLeg = m_legs.First();
                return firstLeg.LoadLocation == @event.Location;
            }

            if (@event.EventType == HandlingEventType.Claim)
            {
                Leg lastLeg = m_legs.Last();
                return lastLeg.UnloadLocation == @event.Location;
            }

            if (@event.EventType == HandlingEventType.Load)
            {
                return m_legs.Any(x => x.LoadLocation == @event.Location);
            }

            if (@event.EventType == HandlingEventType.Unload)
            {
                return m_legs.Any(x => x.UnloadLocation == @event.Location);
            }

            //@event.EventType == HandlingEventType.Customs
            return true;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        private Boolean IsEmpty
        {
            get { return m_legs.Count() == 0; }
        }

        #region Infrastructure
        /// <summary>
        /// To be overridden in inheriting clesses for providing a collection of atomic values of
        /// this Value Object.
        /// </summary>
        /// <returns>Collection of atomic values.</returns>
        protected override IEnumerable<Object> GetAtomicValues()
        {
            foreach (Leg leg in m_legs)
            {
                yield return leg;
            }
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Boolean operator ==(Itinerary left, Itinerary right)
        {
            return EqualOperator(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Boolean operator !=(Itinerary left, Itinerary right)
        {
            return NotEqualOperator(left, right);
        }
        #endregion
    }
}
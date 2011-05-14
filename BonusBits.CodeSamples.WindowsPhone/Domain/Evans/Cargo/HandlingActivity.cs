using System;
using System.Collections.Generic;
using BonusBits.CodeSamples.WP7.Domain.Evans.Handling;

namespace BonusBits.CodeSamples.WP7.Domain.Evans.Cargo
{
    /// <summary>
    /// A handling activity represents how and where a cargo can be handled,
    /// and can be used to express predictions about what is expected to
    /// happen to a cargo in the future.
    /// http://dddsample.sourceforge.net/
    /// </summary>
#pragma warning disable 661,660 //Equals and GetHashCode are overridden in ValueObject class.
    public sealed class HandlingActivity : ValueObject
#pragma warning restore 661,660
    {
        private readonly HandlingEventType m_eventType;
        private readonly Location.Location m_location;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlingActivity"/> class.
        /// </summary>
        public HandlingActivity() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlingActivity"/> class.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="location">The location.</param>
        public HandlingActivity(HandlingEventType eventType, Location.Location location)
        {
            if (location == null)
            {
                throw new ArgumentNullException("location");
            }

            m_eventType = eventType;
            m_location  = location;
        }

        /// <summary>
        /// Gets the type of the event.
        /// </summary>
        /// <value>The type of the event.</value>
        public HandlingEventType EventType
        {
            get { return m_eventType; }
        }

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <value>The location.</value>
        public Location.Location Location
        {
            get { return m_location; }
        }

        /// <summary>
        /// To be overridden in inheriting clesses for providing a collection of atomic values of
        /// this Value Object.
        /// </summary>
        /// <returns>Collection of atomic values.</returns>
        protected override IEnumerable<Object> GetAtomicValues()
        {
            yield return EventType;
            yield return Location.UnLocode;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Boolean operator ==(HandlingActivity left, HandlingActivity right)
        {
            return EqualOperator(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Boolean operator !=(HandlingActivity left, HandlingActivity right)
        {
            return NotEqualOperator(left, right);
        }
    }
}
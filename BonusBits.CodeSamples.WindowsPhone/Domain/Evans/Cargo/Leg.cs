using System;
using System.Collections.Generic;

namespace BonusBits.CodeSamples.WP7.Domain.Evans.Cargo
{
    /// <summary>
    /// Represents one step of an itinerary.
    /// http://dddsample.sourceforge.net/
    /// </summary>
#pragma warning disable 661,660 //Equals and GetHashCode are overridden in ValueObject class.
    public sealed class Leg : ValueObject
#pragma warning restore 661,660
    {
        private readonly Location.Location m_loadLocation;
        private readonly Location.Location m_unloadLocation;

        private readonly DateTime m_loadDate;
        private readonly DateTime m_unloadDate;

        /// <summary>
        /// Initializes a new instance of the <see cref="Leg"/> class.
        /// </summary>
        public Leg() { }

        /// <summary>
        /// Creates new leg instance.
        /// </summary>
        /// <param name="loadLocation">Location where cargo is supposed to be loaded.</param>
        /// <param name="loadDate">Date and time when cargo is supposed to be loaded</param>
        /// <param name="unloadLocation">Location where cargo is supposed to be unloaded.</param>
        /// <param name="unloadDate">Date and time when cargo is supposed to be unloaded.</param>
        public Leg(
            Location.Location loadLocation,
            DateTime loadDate,
            Location.Location unloadLocation,
            DateTime unloadDate)
        {
            m_loadLocation   = loadLocation;
            m_unloadDate     = unloadDate;
            m_unloadLocation = unloadLocation;
            m_loadDate       = loadDate;
        }

        /// <summary>
        /// Gets location where cargo is supposed to be loaded.
        /// </summary>
        public Location.Location LoadLocation
        {
            get { return m_loadLocation; }
        }

        /// <summary>
        /// Gets location where cargo is supposed to be unloaded.
        /// </summary>
        public Location.Location UnloadLocation
        {
            get { return m_unloadLocation; }
        }

        /// <summary>
        /// Gets date and time when cargo is supposed to be loaded.
        /// </summary>
        public DateTime LoadDate
        {
            get { return m_loadDate; }
        }

        /// <summary>
        /// Gets date and time when cargo is supposed to be unloaded.
        /// </summary>
        public DateTime UnloadDate
        {
            get { return m_unloadDate; }
        }

        #region Infrastructure
        protected override IEnumerable<Object> GetAtomicValues()
        {
            yield return m_loadLocation;
            yield return m_unloadLocation;
            yield return m_loadDate;
            yield return m_unloadDate;
        }

        public static Boolean operator ==(Leg left, Leg right)
        {
            return EqualOperator(left, right);
        }

        public static Boolean operator !=(Leg left, Leg right)
        {
            return NotEqualOperator(left, right);
        }
        #endregion
    }
}

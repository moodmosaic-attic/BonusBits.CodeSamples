using System;
using System.Collections.Generic;
using System.Linq;

namespace BonusBits.CodeSamples.WP7.Domain.Evans.Handling
{
    /// <summary>
    /// Contains information about cargo handling history. 
    /// http://dddsample.sourceforge.net/
    /// </summary>
#pragma warning disable 660,661
    public class HandlingHistory : ValueObject
#pragma warning restore 660,661
    {
        private readonly IList<HandlingEvent> m_events;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlingHistory"/> class.
        /// </summary>
        /// <param name="events">The events.</param>
        public HandlingHistory(IEnumerable<HandlingEvent> events)
        {
            m_events = new List<HandlingEvent>(events);
        }

        /// <summary>
        /// Gets a collection of events ordered by their completion time.
        /// </summary>
        public IEnumerable<HandlingEvent> EventsByCompletionTime
        {
            get { return m_events.OrderBy(x => x.CompletionDate); }
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Boolean operator ==(HandlingHistory left, HandlingHistory right)
        {
            return EqualOperator(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Boolean operator !=(HandlingHistory left, HandlingHistory right)
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
            return m_events.Cast<Object>();
        }
    }
}

using System;
using Caliburn.Micro;

namespace BonusBits.CodeSamples.WP7.Domain.Evans.Handling
{
    /// <summary>
    /// Single cargo handling event.
    /// http://dddsample.sourceforge.net/
    /// </summary>   
    public class HandlingEvent
    {
        private readonly IEventAggregator m_eventAggregator;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlingEvent"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        public HandlingEvent(IEventAggregator eventAggregator)
        {
            m_eventAggregator = eventAggregator;
        }

        /// <summary>
        /// Cargo which this handling event is concerned.
        /// </summary>
        public Cargo.Cargo Cargo { get; set; }
        /// <summary>
        /// Type of the event.
        /// </summary>
        public HandlingEventType EventType { get; set; }
        /// <summary>
        /// Location where event occured.
        /// </summary>
        public Location.Location Location { get; set; }
        /// <summary>
        /// Date when event was registered.
        /// </summary>
        public DateTime RegistrationDate { get; set; }
        /// <summary>
        /// Date when action represented by the event was completed.
        /// </summary>
        public DateTime CompletionDate { get; set; }
        /// <summary>
        /// Unique id of this event.
        /// </summary>
        public Guid Id { get; protected set; }

        /// <summary>
        /// Creates new event.
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="location"></param>
        /// <param name="registrationDate"></param>
        /// <param name="completionDate"></param>
        /// <param name="cargo"></param>
        public HandlingEvent(HandlingEventType eventType, Location.Location location,
           DateTime registrationDate, DateTime completionDate, Cargo.Cargo cargo)
        {
            EventType = eventType;
            Location = location;
            RegistrationDate = registrationDate;
            CompletionDate = completionDate;
            Cargo = cargo;

            m_eventAggregator.Publish<CargoWasHandledEvent>(new CargoWasHandledEvent());
        }
    }
}
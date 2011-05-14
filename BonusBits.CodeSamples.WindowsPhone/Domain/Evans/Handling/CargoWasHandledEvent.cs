using System;
using Caliburn.Micro;

namespace BonusBits.CodeSamples.WP7.Domain.Evans.Handling
{
    /// <summary>
    /// Signals that a cargo was handled.
    /// http://dddsample.sourceforge.net/
    /// </summary>
    public sealed class CargoWasHandledEvent : IHandle<HandlingEvent>
    {
        #region IHandle<HandlingEvent> Members

        /// <summary>
        /// Handles the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(HandlingEvent message)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

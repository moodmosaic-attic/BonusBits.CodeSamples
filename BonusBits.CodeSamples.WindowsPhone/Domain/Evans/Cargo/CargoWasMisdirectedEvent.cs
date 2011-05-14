using Caliburn.Micro;

namespace BonusBits.CodeSamples.WP7.Domain.Evans.Cargo
{
    /// <summary>
    /// Raised after cargo has been found to be misdirected.
    /// http://dddsample.sourceforge.net/
    /// </summary>
    public sealed class CargoWasMisdirectedEvent : IHandle<Cargo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CargoWasMisdirectedEvent"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public CargoWasMisdirectedEvent(Cargo source) { }

        #region IHandle<Cargo> Members

        /// <summary>
        /// Handles the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(Cargo message)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}

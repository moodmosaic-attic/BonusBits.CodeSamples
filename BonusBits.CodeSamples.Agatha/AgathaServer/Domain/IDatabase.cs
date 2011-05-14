using System;

namespace BonusBits.CodeSamples.Agatha.Backend.Domain
{
    /// <summary>
    /// http://onestepback.org/index.cgi/Tech/Ruby/DependencyInjectionInRuby.rdoc
    /// </summary>
    public interface IDatabase : IDisposable
    {
        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        ILogger Logger { get; }

        /// <summary>
        /// Gets the error handler.
        /// </summary>
        /// <value>The error handler.</value>
        IErrorHandler ErrorHandler { get; }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        void Close();
    }
}


namespace BonusBits.CodeSamples.Agatha.Backend.Domain
{
    /// <summary>
    /// http://onestepback.org/index.cgi/Tech/Ruby/DependencyInjectionInRuby.rdoc
    /// </summary>
	public interface IStockQuote
	{
        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
		ILogger Logger             { get; }

        /// <summary>
        /// Gets the error handler.
        /// </summary>
        /// <value>The error handler.</value>
		IErrorHandler ErrorHandler { get; }

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>The database.</value>
        IDatabase Database         { get; }
	}
}

namespace BonusBits.CodeSamples.Agatha.Backend.Domain
{
    /// <summary>
    /// http://onestepback.org/index.cgi/Tech/Ruby/DependencyInjectionInRuby.rdoc
    /// </summary>
	public interface IErrorHandler
	{
        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
		ILogger Logger { get; }
	}
}


namespace BonusBits.CodeSamples.Agatha.Backend.Domain
{
    /// <summary>
    /// http://onestepback.org/index.cgi/Tech/Ruby/DependencyInjectionInRuby.rdoc
    /// </summary>
	public interface IWebService
	{
        /// <summary>
        /// Gets the authenticator.
        /// </summary>
        /// <value>The authenticator.</value>
		IAuthenticator Authenticator { get; }

        /// <summary>
        /// Gets the stock quote.
        /// </summary>
        /// <value>The stock quote.</value>
		IStockQuote StockQuote       { get; }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        void Execute();
	}
}
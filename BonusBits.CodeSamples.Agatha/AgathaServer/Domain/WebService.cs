
namespace BonusBits.CodeSamples.Agatha.Backend.Domain
{
    /// <summary>
    /// http://onestepback.org/index.cgi/Tech/Ruby/DependencyInjectionInRuby.rdoc
    /// </summary>
	public sealed class WebService : IWebService
	{
		private readonly IAuthenticator m_authenticator;
        private readonly IStockQuote    m_quotes;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebService"/> class.
        /// </summary>
        /// <param name="authenticator">The authenticator.</param>
        /// <param name="quotes">The quotes.</param>
		public WebService(IAuthenticator authenticator, IStockQuote quotes)
		{
			m_authenticator = authenticator;
			m_quotes        = quotes;
		}

        /// <summary>
        /// Gets the authenticator.
        /// </summary>
        /// <value>The authenticator.</value>
		public IAuthenticator Authenticator { get { return m_authenticator; } }

        /// <summary>
        /// Gets the stock quote.
        /// </summary>
        /// <value>The stock quote.</value>
		public IStockQuote StockQuote { get { return m_quotes; } }

        public void Execute() { }
    }
}
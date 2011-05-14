namespace BonusBits.CodeSamples.Agatha.Backend.Domain
{
    /// <summary>
    /// http://onestepback.org/index.cgi/Tech/Ruby/DependencyInjectionInRuby.rdoc
    /// </summary>
	public sealed class Authenticator : IAuthenticator
	{
		private readonly ILogger       m_logger;
        private readonly IErrorHandler m_handler;
        private readonly IDatabase     m_database;

        /// <summary>
        /// Initializes a new instance of the <see cref="Authenticator"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="handler">The handler.</param>
        /// <param name="database">The database.</param>
		public Authenticator(ILogger logger, IErrorHandler handler, IDatabase database)
		{
            m_logger   = logger;
            m_handler  = handler;
            m_database = database;
		}

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        public ILogger Logger { get { return m_logger; } }

        /// <summary>
        /// Gets the error handler.
        /// </summary>
        /// <value>The error handler.</value>
        public IErrorHandler ErrorHandler { get { return m_handler; } }

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>The database.</value>
        public IDatabase Database { get { return m_database; } }
	}
}
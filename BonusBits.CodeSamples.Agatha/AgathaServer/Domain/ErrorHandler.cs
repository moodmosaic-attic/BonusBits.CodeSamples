namespace BonusBits.CodeSamples.Agatha.Backend.Domain
{   
    /// <summary>
    /// http://onestepback.org/index.cgi/Tech/Ruby/DependencyInjectionInRuby.rdoc
    /// </summary>
    public sealed class ErrorHandler : IErrorHandler
    {
        private readonly ILogger m_logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorHandler"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ErrorHandler(ILogger logger)
        {
            m_logger = logger;
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        public ILogger Logger { get { return m_logger; } }
    }
}

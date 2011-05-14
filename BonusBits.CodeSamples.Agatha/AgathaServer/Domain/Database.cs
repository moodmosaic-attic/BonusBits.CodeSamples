using System;
namespace BonusBits.CodeSamples.Agatha.Backend.Domain
{
    /// <summary>
    /// http://onestepback.org/index.cgi/Tech/Ruby/DependencyInjectionInRuby.rdoc
    /// </summary>
    public sealed class Database : IDatabase
    {
        private readonly ILogger m_logger;
        private readonly IErrorHandler m_handler;

        public Database(ILogger logger, IErrorHandler handler)
        {
            m_logger  = logger;
            m_handler = handler;

            Console.WriteLine("In Database .ctor");
        }

        // This public method can be called instead of Dispose.
        public void Close()
        {
            Dispose(true);
        }

        // When garbage collected, this Finalize method runs to close the resource.
        ~Database()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("In Database finalizer.");
            Console.ResetColor();

            // Call the method that actually does the cleanup.
            Dispose(false);
        }

        // This is the common method that does the actual cleanup.  
        // Finalize, Dispose, and Close call this method.   
        private void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                // The object is being explicitly disposed/closed, not   
                // finalized. It is therefore safe for code in this if   
                // statement to access fields that reference other   
                // objects because the Finalize method of these other objects   
                // hasn't yet been called.
            }

            GC.SuppressFinalize(this);
        }

        #region IDatabase Members

        public ILogger Logger
        {
            get { return m_logger; }
        }

        public IErrorHandler ErrorHandler
        {
            get { return m_handler; }
        }

        #endregion

        #region IDisposable Members

        // This public method can be called to deterministically close  
        // the resource. This method implements IDisposable's Dispose.
        public void Dispose()
        {
            // Call the method that actually does the cleanup.  
            Dispose(true);
        }

        #endregion
    }
}

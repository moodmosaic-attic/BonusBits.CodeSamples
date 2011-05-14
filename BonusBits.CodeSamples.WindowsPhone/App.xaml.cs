using System;
using System.Windows;
using Wintellect.Sterling;

namespace BonusBits.CodeSamples.WP7
{
    public partial class App : Application
    {
        private static ISterlingDatabaseInstance s_database;
        private static SterlingEngine            s_engine;
        private static SterlingDefaultLogger     s_logger;

        public App()
        {
            InitializeComponent();

            UnhandledException += (_, e) => { HandleApplicationEnd(_, EventArgs.Empty); };

            Startup += HandleApplicationStartup;
            Exit    += HandleApplicationEnd;
        }

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>The database.</value>
        public static ISterlingDatabaseInstance Database
        {
            get
            {
                return s_database;
            }
        }

        /// <summary>
        /// Handles the application startup event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.StartupEventArgs"/> 
        /// instance containing the event data.</param>
        private static void HandleApplicationStartup(Object sender, StartupEventArgs e)
        {
            s_engine = new SterlingEngine();
            s_logger = new SterlingDefaultLogger(SterlingLogLevel.Information);
            s_engine.Activate();
            s_database = s_engine.SterlingDatabase.RegisterDatabase<AppDatabase>();
        }

        /// <summary>
        /// Handles the application end event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> 
        /// instance containing the event data.</param>
        private static void HandleApplicationEnd(Object sender, EventArgs e)
        {
            s_logger.Detach();
            s_engine.Dispose();
            s_database = null;
            s_engine   = null;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using BonusBits.CodeSamples.WP7.Domain.Common;
using Caliburn.Micro;
using Wintellect.Threading.AsyncProgModel;

namespace BonusBits.CodeSamples.WP7
{
    public sealed class ThreadingPageViewModel : PropertyChangedBase
    {
        private const Int32 c_iterations = 5000;

        private readonly INavigationService m_navigator;
        private readonly IWebService        m_webService;

        private Int32   m_numDone;
        private Boolean m_canStart;
        private String  m_status;

        // Sets the animation's visibility.
        private Boolean m_canExecuteWithSyncIO;

        private enum StatusState { Busy, Ready };

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPageViewModel"/> class.
        /// </summary>
        public ThreadingPageViewModel(INavigationService navigator)
        {
            // INavigationService is available to us using Constructor Injection.
            m_navigator = navigator;

            // HACK: The following call to instantiate the m_svc is the opposite of 
            // Inversion of Control, also known as Direct Control and is considered
            // a DI Anti-Pattern. This happens every time we create a new instance 
            // of a type by using the new keyword.
            m_webService = new WebService(new StockQuote());

            SetStatus(String.Empty, StatusState.Ready);
        }

        #region ViewModel Properties
        /// <summary>
        /// Gets or sets a value indicating whether this instance can execute with sync IO.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can execute with sync IO; otherwise, <c>false</c>.
        /// </value>
        public Boolean CanExecuteWithSyncIO
        {
            get
            {
                return m_canExecuteWithSyncIO;
            }
            set
            {
                m_canExecuteWithSyncIO = value;
                NotifyOfPropertyChange(() => CanExecuteWithSyncIO);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can execute with delegate begin
        /// invoke.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can execute with delegate begin invoke; otherwise, 
        /// 	<c>false</c>.
        /// </value>
        public Boolean CanExecuteWithDelegateBeginInvoke { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can execute with IAsyncResult APM.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can execute with I async result; otherwise, <c>false</c>.
        /// </value>
        public Boolean CanExecuteWithIAsyncResult { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can execute with AsyncEnumerator.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can execute with async enumerator; otherwise, <c>false</c>.
        /// </value>
        public Boolean CanExecuteWithAsyncEnumerator { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the start button can be enabled or not.
        /// </summary>
        /// <value><c>true</c> the start button can be enabled; otherwise, <c>false</c>.</value>
        public Boolean CanStart
        {
            get { return m_canStart; }
            set
            {
                m_canStart = value;
                NotifyOfPropertyChange(() => CanStart);
            }
        }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public String Status
        {
            get
            {
                return m_status;
            }

            set
            {
                m_status = value;
                NotifyOfPropertyChange(() => Status);
            }
        }
        #endregion

        #region ViewModel Commands
        public void Start()
        {
            NotifyOfPropertyChange(() => CanExecuteWithSyncIO);

            if (CanExecuteWithSyncIO)
            {
                MessageBox.Show("UI will pause while Sync I/O executes.");

                ExecuteWithSyncIO();
            }

            if (CanExecuteWithDelegateBeginInvoke)
            {
                try { ExecuteWithDelegateBeginInvoke(); }
                catch (NotSupportedException e)
                {
                    SetStatus(String.Empty, StatusState.Ready, e.Message);
                }
            }

            if (CanExecuteWithIAsyncResult)
            {
                ExecuteWithIAsyncResult();
            }

            if (CanExecuteWithAsyncEnumerator)
            {
                ExecuteWithAsyncEnumerator();
            }
        }  
        #endregion

        #region Sync I/O
        private void ExecuteWithSyncIO()
        {
            for (Int32 n = 0; n < c_iterations; n++)
            {
                m_webService.FetchStockQuotes();
            }

            SetStatus("Sync/IO completed.", StatusState.Ready);
        }
        #endregion

        #region Delegate's BeginInvoke
        private void ExecuteWithDelegateBeginInvoke()
        {
            Func<IStockQuote> stockQuoteDelegate = m_webService.FetchStockQuotes;
            // NOTE: Calling delegates asynchronously is NOT supported in Windows Phone 7.
            stockQuoteDelegate.BeginInvoke((ar) =>
            {
                stockQuoteDelegate.EndInvoke(ar);
            }, null);
        }
        #endregion

        #region IAsyncResult APM
        private void ExecuteWithIAsyncResult()
        {
            SetStatus("Working..", StatusState.Busy);

            for (Int32 n = 0; n < c_iterations; n++)
            {
                ((WebService)m_webService).BeginGetStockQuotes((ar) =>
                {
                    if (Interlocked.Increment(ref m_numDone) == c_iterations)
                    {
                        Execute.OnUIThread(() =>
                        {
                            SetStatus("IAsyncResult APM completed.", StatusState.Ready);
                        });
                    }
                }, null);
            }
        }
        #endregion
        
        #region AsyncEnumerator
        private void ExecuteWithAsyncEnumerator()
        {
            SetStatus("Working..", StatusState.Busy);
            
            AsyncEnumerator ae = new AsyncEnumerator();
            ae.BeginExecute(ExecuteWithAsyncEnumerator(ae), ae.EndExecute, null);
        }

        private IEnumerator<Int32> ExecuteWithAsyncEnumerator(AsyncEnumerator ae)
        {
            for (Int32 n = 0; n < c_iterations; n++)
            {
                ((WebService)m_webService).BeginGetStockQuotes(ae.End(), null);
            }

            // NOTE: AsyncEnumerator captures the calling thread's SynchronizationContext.
            // Set the Wintellect.Threading.AsyncProgModel.SynchronizationContext type to
            // null so that the callback continues on a ThreadPool thread.
            ae.SyncContext = null;

            yield return c_iterations;

            for (Int32 n = 0; n < c_iterations; n++)
            {
                ((WebService)m_webService).EndGetStockQuotes(ae.DequeueAsyncResult());
            }

            // AsyncEnumerator captures the synchronization context.
            SetStatus("AsyncEnumerator completed.", StatusState.Ready);
        }
        #endregion

        /// <summary>
        /// Sets the status.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="state">The state.</param>
        /// <param name="messageBoxText">The message box text.</param>
        private void SetStatus(String message, StatusState state, String messageBoxText = null)
        {
            m_numDone = 0;
            Status    = message;
            CanStart  = state == StatusState.Ready ? true : false;

            if (!String.IsNullOrEmpty(messageBoxText))
            {
                MessageBox.Show(messageBoxText);
            }
        }
    }
}
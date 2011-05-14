using System;
using System.Threading;
using Wintellect.Threading.AsyncProgModel;

namespace BonusBits.CodeSamples.WP7.Domain.Common
{
    internal sealed class WebService : IWebService
    {
        private readonly IStockQuote m_quotes;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebService"/> class.
        /// </summary>
        /// <param name="quotes">The quotes.</param>
        public WebService(IStockQuote quotes)
        {
            m_quotes = quotes;
        }

        // Asynchronous version of time-consuming method (Begin part).
        public IAsyncResult BeginGetStockQuotes(AsyncCallback callback, Object state)
        {
            // Create IAsyncResult Object identifying the 
            // asynchronous operation.
            AsyncResult<IStockQuote> ar = new AsyncResult<IStockQuote>(callback, state);

            // Use a thread pool thread to perform the operation.
            ThreadPool.QueueUserWorkItem(GetStockQuotesHelper, ar);

            return ar; // Return the IAsyncResult to the caller.
        }

        // Asynchronous version of time-consuming method (End part).
        public IStockQuote EndGetStockQuotes(IAsyncResult asyncResult)
        {
            // We know that the IAsyncResult is really an 
            // AsyncResult<IStockQuote> object.
            AsyncResult<IStockQuote> ar = (AsyncResult<IStockQuote>)asyncResult;

            // Wait for operation to complete, then return result or 
            // throw exception.
            return ar.EndInvoke();
        }

        private void GetStockQuotesHelper(Object state)
        {
            // We know that it's really an AsyncResult<IStockQuote> object.
            AsyncResult<IStockQuote> ar = (AsyncResult<IStockQuote>)state;
            try
            {
                // Perform the operation; if sucessful set the result.
                IStockQuote quotes = FetchStockQuotes();
                ar.SetAsCompleted(quotes, false);
            }
            catch (Exception e)
            {
                // If operation fails, set the exception.
                ar.SetAsCompleted(e, false);
            }
        }

        #region IWebService Members

        /// <summary>
        /// Gets the stock quotes.
        /// </summary>
        /// <returns></returns>
        public IStockQuote FetchStockQuotes()
        {
            Thread.Sleep(5); // Simulate time-consuming task.
            return m_quotes;
        }

        #endregion
    }
}

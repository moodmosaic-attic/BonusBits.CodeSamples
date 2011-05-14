using System;
using System.Threading;
using System.Threading.Tasks;

#if AsyncEnumerator
using Wintellect.Threading.AsyncProgModel;
#endif

namespace BonusBits.CodeSamples.Async.Domain
{
    /// <summary>
    /// A class written with .NET 2.0 to demonstrate the APM.
    /// </summary>
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
#if AsyncEnumerator
        // Asynchronous version of time-consuming method (Begin part).
        public IAsyncResult BeginFetchStockQuotes(AsyncCallback callback, Object state)
        {
            // Create IAsyncResult Object identifying the 
            // asynchronous operation.
            AsyncResult<IStockQuote> ar = new AsyncResult<IStockQuote>(callback, state);

            // Use a thread pool thread to perform the operation.
            ThreadPool.QueueUserWorkItem(FetchStockQuotesHelper, ar);

            return ar; // Return the IAsyncResult to the caller.
        }

        // Asynchronous version of time-consuming method (End part).
        public IStockQuote EndFetchStockQuotes(IAsyncResult asyncResult)
        {
            // We know that the IAsyncResult is really an 
            // AsyncResult<IStockQuote> object.
            AsyncResult<IStockQuote> ar = (AsyncResult<IStockQuote>)asyncResult;

            // Wait for operation to complete, then return result or 
            // throw exception.
            return ar.EndInvoke();
        }

        private void FetchStockQuotesHelper(Object state)
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
                // If operation fails, set the exception
                ar.SetAsCompleted(e, false);
            }
        }
#endif
        #region IWebService Members
        /// <summary>
        /// Gets the stock quotes.
        /// </summary>
        /// <returns></returns>
        public IStockQuote FetchStockQuotes()
        {
            Thread.Sleep(5000); // Simulate time-consuming task.
            return m_quotes;
        }
        #endregion
    }

    /// <summary>
    /// Extension Methods for converting the APM to a Task thus allowing us
    /// to use it with the Async CTP.
    /// </summary>
    public static class WebServiceExtensions
    {
        public static Task<IStockQuote> FetchStockQuotesTaskAsync(this IWebService svc)
        {
#if AsyncEnumerator
            return Task<IStockQuote>.Factory.FromAsync(
                ((WebService)svc).BeginFetchStockQuotes,
                ((WebService)svc).EndFetchStockQuotes,
                null);
#else
            // Just for the demo we assume the user has not an IAsyncResult
            // interface implementation ready for the WebService class, thus
            // we are invoking the delegate async,  ignoring the known perf 
            // hit when calling delegates asynchronously.
            Func<IStockQuote> @delegate = svc.FetchStockQuotes;
            return Task.Factory.FromAsync<IStockQuote>(
                @delegate.BeginInvoke,
                @delegate.EndInvoke,
                null);
#endif
        }
    }
}


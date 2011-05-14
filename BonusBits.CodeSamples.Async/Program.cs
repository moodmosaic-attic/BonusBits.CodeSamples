using System;
using System.Collections.Generic;
using BonusBits.CodeSamples.Async.Domain;

#if AsyncEnumerator
using Wintellect.Threading.AsyncProgModel;
#endif

namespace BonusBits.CodeSamples.Async
{
    internal sealed class Program
    {
        private static void Main()
        {
            IStockQuote qt = new StockQuote();
            WebService svc = new WebService(qt);

            FetchStockQuotesSync(svc);
            FetchStockQuotesApm(svc);
#if AsyncEnumerator
            FetchStockQuotesAsyncEnumerator(svc);
#endif
            FetchStockQuotesAsyncCtp(svc);

            Console.ReadLine();
        }

        // Sync.

        private static void FetchStockQuotesSync(WebService svc)
        {
            // This blocks. You don't know when the FetchStockQuotes
            // method returns. It may takes from minutes, to hours or
            // it may not return at all.
            IStockQuote qt = svc.FetchStockQuotes();
        }

        // IAsyncResult interface (APM).
#if AsyncEnumerator
        private static void FetchStockQuotesApm(WebService svc)
        {
            // This never blocks. Your code returns immediately.
            svc.BeginFetchStockQuotes(FetchStockQuotesApmCallback, svc);
        }

        private static void FetchStockQuotesApmCallback(IAsyncResult ar)
        {
            // This never blocks. Your code returns immediately.
            WebService svc = (WebService)ar.AsyncState;
            IStockQuote qt = svc.EndFetchStockQuotes(ar);
        }
#else
        private static void FetchStockQuotesApm(WebService svc)
        {
            Func<IStockQuote> @delegate = svc.FetchStockQuotes; 
            // This never blocks. Your code returns immediately.
            @delegate.BeginInvoke(FetchStockQuotesApmCallback, @delegate);
        }

        private static void FetchStockQuotesApmCallback(IAsyncResult ar)
        {
            // This never blocks. Your code returns immediately.
            Func<IStockQuote> @delegate = (Func<IStockQuote>)ar.AsyncState;
            IStockQuote  qt = @delegate.EndInvoke(ar);
        }
#endif
#if AsyncEnumerator
        // AsyncEnumerator class.

        private static void FetchStockQuotesAsyncEnumerator(WebService svc)
        {
            AsyncEnumerator ae = new AsyncEnumerator();
            ae.BeginExecute(FetchStockQuotesAsyncEnumerator(ae, svc), ae.EndExecute);
        }

        private static IEnumerator<Int32> FetchStockQuotesAsyncEnumerator(AsyncEnumerator ae, WebService svc)
        {
            svc.BeginFetchStockQuotes(ae.End(), null);
            yield return 1;
            IStockQuote qt = svc.EndFetchStockQuotes(ae.DequeueAsyncResult());
        }
#endif
        // Async CTP

        private static async void FetchStockQuotesAsyncCtp(WebService svc)
        {
            IStockQuote qt = await svc.FetchStockQuotesTaskAsync();
        }
    }
}

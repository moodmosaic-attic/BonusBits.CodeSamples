using System;

namespace BonusBits.CodeSamples.Async.Domain
{
    public interface IWebService
    {
        IStockQuote FetchStockQuotes();
    }
}

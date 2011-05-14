using System;
using System.Threading;
using Wintellect.Sterling;
using Wintellect.Threading.AsyncProgModel;

namespace BonusBits.CodeSamples.WP7.Infrastructure.Threading
{
    public static class SterlingCallbackBased
    {
        /// <summary> 
        /// Asynchronous version of ISterlingDatabaseInstance Save method (Begin part). 
        /// </summary>
        public static IAsyncResult BeginSave<T>(
            this ISterlingDatabaseInstance sterling,
            T instance,
            AsyncCallback callback,
            Object state) where T : class, new()
        {
            // Create IAsyncResult Object identifying the asynchronous operation.
            AsyncResult ar = new AsyncResult(callback, state);

            // Use a thread pool thread to perform the operation.
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                var asyncResult = (AsyncResult)obj;
                try
                {
                    // Perform the operation.
                    sterling.Save<T>(instance);
                    asyncResult.SetAsCompleted(null, false);
                }
                catch (Exception e)
                {
                    // If operation fails, set the exception.
                    asyncResult.SetAsCompleted(e, false);
                }
            }, ar);

            return ar; // Return the IAsyncResult to the caller.
        }

        ///<summary>
        /// Asynchronous version of ISterlingDatabaseInstance Save method (End part).
        /// </summary>
        public static void EndSave(this ISterlingDatabaseInstance instance, IAsyncResult asyncResult)
        {
            AsyncResult ar = (AsyncResult)asyncResult;
            ar.EndInvoke();
        }
    }
}

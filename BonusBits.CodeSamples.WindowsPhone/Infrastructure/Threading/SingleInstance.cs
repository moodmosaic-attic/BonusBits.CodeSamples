using System;
using System.Threading;

namespace BonusBits.CodeSamples.WP7.Infrastructure.Threading
{
    internal sealed class SingleInstance<T> where T : class
    {
        private readonly Object  m_lockObj = new Object();
        private readonly Func<T> m_delegate;
        private Boolean m_isDelegateInvoked;
        
        private T m_value;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="SingleInstance&lt;T&gt;"/> class.
        /// </summary>
        public SingleInstance()
            : this(() => default(T)) { }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="SingleInstance&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="delegate">The @delegate.</param>
        public SingleInstance(Func<T> @delegate)
        {
            m_delegate = @delegate;
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public T Instance
        {
            get
            {
                if (!m_isDelegateInvoked)
                {
                    T temp = m_delegate();
                    Interlocked.CompareExchange<T>(ref m_value, temp, null);

                    Boolean lockTaken = false;

                    try
                    {
                        // WP7 does not support the overload with the
                        // Boolean indicating if the lock was taken.
                        Monitor.Enter(m_lockObj); lockTaken = true;

                        m_isDelegateInvoked = true;
                    }
                    finally
                    {
                        if (lockTaken) { Monitor.Exit(m_lockObj); }
                    }
                }

                return m_value;
            }
        }
    }
}
using System;
using System.Threading;

namespace TicketProvider.Net
{
    public class AsyncResultVoid : IAsyncResult
    {

        #region Variables

        private readonly AsyncCallback _asyncCallback;
        private readonly object _asyncState;

        private enum AsyncResultCompleteState : int
        {
            Pending,
            CompletedSynchronously,
            CompletedAsynchronously
        }

        private Int32 _completeState;
        private ManualResetEvent _asyncWaitHandle;
        private Exception _ex;

        #endregion

        #region Properties

        public object AsyncState
        {
            get { return _asyncState; }
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                if (_asyncWaitHandle == null)
                {

                    bool isCompleted;
                    ManualResetEvent resetEvent;

                    // Create a new wait handle with the correct state.
                    isCompleted = IsCompleted;
                    resetEvent = new ManualResetEvent(isCompleted);
                    if (Interlocked.CompareExchange(ref _asyncWaitHandle,
                       resetEvent, null) != null)
                    {

                        // Another thread created this async results's wait event. Dispose the
                        // just created event.
                        resetEvent.Close();
                        resetEvent = null;
                    }
                    else
                    {

                        // Check if the operation was completed while creating the
                        // wait event.
                        if (!isCompleted && IsCompleted)
                            _asyncWaitHandle.Set();
                    }
                }

                return _asyncWaitHandle;
            }
        }

        public bool CompletedSynchronously
        {
            get { return (Thread.VolatileRead(ref _completeState) == (Int32)AsyncResultCompleteState.CompletedSynchronously); }
        }

        public bool IsCompleted
        {
            get { return (Thread.VolatileRead(ref _completeState) != (Int32)AsyncResultCompleteState.Pending); }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new instance of the AsyncResultNoResult class using the specified parameters.
        /// </summary>
        /// <param name="asyncCallback">Optional, may be null.</param>
        /// <param name="state">Optional, may be null.</param>
        public AsyncResultVoid(AsyncCallback asyncCallback, object state)
        {
            _asyncCallback = asyncCallback;
            _asyncState = state;
        }

        /// <summary>
        /// Sets the operation as completed, optionally setting an asynchronously thrown exception.
        /// Passin null to indicate no exception was thrown.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="completedSynchronously"></param>
        public void Complete(Exception ex, bool completedSynchronously)
        {

            int state;

            // Store the asynchronously thrown exception and verify the operation is not
            // completed more than once.
            _ex = ex;
            state = Interlocked.Exchange(ref _completeState,
                completedSynchronously ? (Int32)AsyncResultCompleteState.CompletedSynchronously : (Int32)AsyncResultCompleteState.CompletedAsynchronously);
            if (state != (Int32)AsyncResultCompleteState.Pending)
                throw new InvalidOperationException("The operation cannot be completed more than once");

            // Set the wait event
            if (_asyncWaitHandle != null)
                _asyncWaitHandle.Set();

            // Invoke callback
            if (_asyncCallback != null)
                _asyncCallback(this);

        }

        /// <summary>
        /// Waits for completion of the operation.
        /// </summary>
        public void EndInvoke()
        {
            if (!IsCompleted)
            {
                AsyncWaitHandle.WaitOne();
                AsyncWaitHandle.Close();
                _asyncWaitHandle = null;  // Allow early GC
            }

            // Throw asynchronously thrown exception.
            if (_ex != null) 
                throw _ex;
        }


        #endregion
    }
}

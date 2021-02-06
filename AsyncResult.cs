using System;
using System.Collections.Generic;
using System.Text;

namespace TicketProvider.Net
{
    public class AsyncResult<T> : AsyncResultVoid
    {

        #region Variables

        private T _result;

        #endregion

        #region Properties

        #endregion

        #region Functions

        public AsyncResult(AsyncCallback asyncCallback, object state)
            : base(asyncCallback, state)
        {
        }

        public void Complete(T result, bool completedSynchronously)
        {

            _result = result;
            base.Complete(null, completedSynchronously);
        }

        public new T EndInvoke()
        {
            base.EndInvoke();
            return _result;
        }

        #endregion
    }
}

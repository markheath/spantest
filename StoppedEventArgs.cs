using System;

namespace SpanTest
{
    public class StoppedEventArgs : EventArgs
    {
        public StoppedEventArgs(Exception exception = null)
        {
            this.Exception = exception;
        }

        public Exception Exception { get; }
    }
}
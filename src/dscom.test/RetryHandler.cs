namespace dSPACE.Runtime.InteropServices.Tests;

internal static class RetryHandler
{
    internal const int DefaultRetryAttempts = 5;
    internal const int DefaultDelayBetweenAttemptsInMilliseconds = 500;

    internal static void Retry(
        Action action,
        IEnumerable<Type> expectedExceptions,
        int retryAttempts = DefaultRetryAttempts,
        int retryDelayInMilliseconds = DefaultDelayBetweenAttemptsInMilliseconds)
    {
        for (var attempts = 0; attempts <= retryAttempts; attempts++)
        {
            try
            {
                action();
                break;
            }
            catch (Exception ex)
            {
                if (!expectedExceptions.Any(x => ex.GetType() == x))
                {
                    throw new RetryAbortException("Unexpected exception in a retry logic.", ex);
                }

                if (attempts >= retryAttempts)
                {
                    throw new RetryAbortException($"Exceeded the maximum attempts of {retryAttempts}.", ex);
                }

                Thread.Sleep(retryDelayInMilliseconds);
            }
        }
    }
}

internal class RetryAbortException : Exception
{ 
    public RetryAbortException(string message, Exception innerException) : base(message, innerException) { }
}

using System.Net;

namespace CollectionHub.Components.Services
{
    public class ApiRetryHandler
    {
        private const int MaxRetries = 3;

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
        {
            for (int attempt = 1; attempt < MaxRetries; attempt++)
            {
                try
                {
                    return await action();
                }
                catch (HttpRequestException exception)
                    when (IsTransient(exception))
                {
                    await Task.Delay(1000 * attempt);
                }
            }

            return await action();
        }

        private static bool IsTransient(
            HttpRequestException exception)
        {
            if (exception.StatusCode is null)
            {
                return true;
            }

            return exception.StatusCode is
                       HttpStatusCode.RequestTimeout or
                       HttpStatusCode.TooManyRequests
                   || (int)exception.StatusCode >= 500;
        }
    }
}
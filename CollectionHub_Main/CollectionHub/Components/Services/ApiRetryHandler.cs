namespace CollectionHub.Components.Services
{
    public class ApiRetryHandler
    {
        private readonly int _maxRetries = 3;

    public async Task<T?> ExecuteAsync<T>(Func<Task<T>> action)
    {
        for (int attempt = 1; attempt <= _maxRetries; attempt++)
        {
            try
            {
                return await action();
            }
            catch (HttpRequestException) when (attempt < _maxRetries)
            {
                await Task.Delay(1000 * attempt);
            }
        }

        throw new HttpRequestException("API request failed after retries.");
    }
    }
}

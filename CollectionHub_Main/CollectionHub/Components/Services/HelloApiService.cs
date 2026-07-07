namespace CollectionHub.Components.Services
{
    public class HelloApiService
    {
        private readonly HttpClient _httpClient;

        public HelloApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetHelloMessageAsync()
        {
            var response = await _httpClient.GetAsync("http://localhost:7029/api/HelloCollectionHubFunction");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}

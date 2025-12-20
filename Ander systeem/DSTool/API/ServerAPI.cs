using System.Net.Http.Json;

namespace LeapDataScienceTool.API
{
    public interface IServerAPI
    {
        HttpClient UseServerApi();
        Task<T?> Get<T>(string endpoint);
        Task<IEnumerable<T>> GetAll<T>(string endpoint);
        Task<T?> Post<T>(string endpoint, object PostDto);
        Task<T?> Put<T>(string endpoint, object updateDto);

        Task<bool> Delete(string endpoint);
    }

    public class ServerAPI : IServerAPI
    {
        private readonly HttpClient client;

        public ServerAPI(IHttpClientFactory httpClient)
        {
            client = httpClient.CreateClient("ServerClient");
        }

        public HttpClient UseServerApi()
        {
            return client;
        }

        public async Task<T?> Get<T>(string endpoint)
        {
            var result = await client.GetAsync(endpoint);
            if (!result.IsSuccessStatusCode)
            {
                return default;
            }
            return await result.Content.ReadFromJsonAsync<T>();
        }

        public async Task<IEnumerable<T>> GetAll<T>(string endpoint)
        {
            var result = await client.GetAsync(endpoint);
            if (!result.IsSuccessStatusCode)
            {
                return [];
            }
            return await result.Content.ReadFromJsonAsync<IEnumerable<T>>();
        }

        public async Task<T?> Post<T>(string endpoint, object PostDto)
        {
            var result = await client.PostAsJsonAsync(endpoint, PostDto);
            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadFromJsonAsync<T>();
            }
            return default;
        }

        public async Task<T?> Put<T>(string endpoint, object updateDto)
        {
            var result = await client.PutAsJsonAsync(endpoint, updateDto);
            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadFromJsonAsync<T>();
            }
            return await result.Content.ReadFromJsonAsync<T>();
        }

        public async Task<bool> Delete(string endpoint)
        {
            var result = await client.DeleteAsync(endpoint);
            return result.IsSuccessStatusCode;
        }
    }
}

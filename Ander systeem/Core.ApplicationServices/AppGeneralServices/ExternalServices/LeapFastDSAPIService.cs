using Leap.ApplicationServices.DTO.DataConfig;
using Leap.ApplicationServices.DTO.DataResult;
using Leap.ApplicationServices.DTO.ModelingProcess;
using Leap.ApplicationServices.Interfaces.ExternalServiceAPI;
using System.Net.Http.Json;
using System.Text.Json;

namespace Leap.ApplicationServices.AppGeneralServices.ExternalServices
{
    public class LeapFastDSAPIService : IPythonFacadeService
    {
        private readonly HttpClient httpClient;
        private readonly JsonSerializerOptions options;

        public LeapFastDSAPIService(IHttpClientFactory httpClientFactory)
        {
            // Initialize any required services or configurations here
            httpClient = httpClientFactory.CreateClient("Leap_PythonService");
            options = new JsonSerializerOptions { PropertyNamingPolicy = null };
        }

        // Requests preview data from Python service
        public async Task<PreviewDataDTO?> RequestPreviewData(DataRequestDTO request)
        {

            // Implement the logic to call the Python service and retrieve preview data.
            var result = await httpClient.PostAsJsonAsync("/data-operation/preview", request, options);
            // Check if the request was successful
            if (!result.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Failed to retrieve preview data: {result.ReasonPhrase}");
            }
            // Deserialize the response to PreviewDataDTO
            return await result.Content.ReadFromJsonAsync<PreviewDataDTO>();
        }

        public async Task<ModelStorageDTO?> StoreModel(ModelRequestDTO request)
        {
            // Implement the logic to call the Python service and retrieve preview data.
            var result = await httpClient.PostAsJsonAsync("/model-operation/model-storage", request, options);
            // Check if the request was successful
            if (!result.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Failed to train and store model: {result.ReasonPhrase}");
            }
            // Deserialize the response to PreviewDataDTO
            return await result.Content.ReadFromJsonAsync<ModelStorageDTO>();
        }

        // Will trigger model training on Python service, without model storage
        public async Task<ModelResultDataDTO?> TriggerModelTraining(ModelRequestDTO request)
        {

            var result = await httpClient.PostAsJsonAsync("/model-operation/preview", request, options);
            if (!result.IsSuccessStatusCode)
            {
                Console.WriteLine(await result.Content.ReadAsStringAsync());
                throw new InvalidOperationException($"Failed train model. {result.StatusCode}");
            }
            return await result.Content.ReadFromJsonAsync<ModelResultDataDTO>();
        }

        public async Task DeleteModel(string ModelAddress)
        {
            var result = await httpClient.DeleteAsync($"/model-operation/remove/{ModelAddress}");
            if (!result.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Failed to delete model with the address {ModelAddress}");
            }
            return;
        }
    }
}

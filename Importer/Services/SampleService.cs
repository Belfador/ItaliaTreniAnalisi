using Importer.Models;
using System.Text;
using System.Text.Json;

namespace Importer.Services
{
    internal class SampleService
    {
        private const int batchSize = 250000;
        private static SampleService? instance;
        public static SampleService Instance => instance ??= new SampleService();

        private SampleService()
        {

        }

        public async Task ImportSamplesAsync(IEnumerable<Sample> samples)
        {
            var requestURI = $"{Resources.apiBaseUrl}/Analysis/importSamples";

            using (var client = new HttpClient())
            {
                int samplesCount = samples.Count();

                for (int i = 0; i < samplesCount; i += batchSize)
                {
                    var batch = samples.Skip(i).Take(batchSize);
                    await PostBatchAsync(client, requestURI, batch);
                }
            }
        }

        private static async Task PostBatchAsync<T>(HttpClient client, string requestURI, T batch)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, requestURI);
            var content = new StringContent(JsonSerializer.Serialize(batch), Encoding.UTF8, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                Guid jobId = JsonSerializer.Deserialize<Guid>(await response.Content.ReadAsStringAsync());

                while (!await GetJobStatusAsync(client, jobId))
                {
                    await Task.Delay(1000);
                }
            }
        }

        private static async Task<bool> GetJobStatusAsync(HttpClient client, Guid jobId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{Resources.apiBaseUrl}/Analysis/status/{jobId}");
            var response = await client.SendAsync(request);
            Job? job = JsonSerializer.Deserialize<Job>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true});

            if (job is null)
            {
                throw new Exception("Job not found.");
            }

            if (job.IsCompleted)
            {
                Logger.Instance.Log($"Job completed: {job.Result}");
                return true;
            }
            else
            {
                Logger.Instance.Log($"Job in progress: ({job.Progress}%)");
                return false;
            }
        }
    }
}

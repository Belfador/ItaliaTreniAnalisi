using Importer.Models;
using System.Text;
using System.Text.Json;

namespace Importer.Services
{
    internal class SampleService
    {
        private const int batchSize = 100000;
        private static SampleService? instance;
        public static SampleService Instance => instance ??= new SampleService();

        private SampleService()
        {

        }

        public async Task ImportAsync(IEnumerable<Sample> samples)
        {
            using (var client = new HttpClient())
            {
                int samplesCount = samples.Count();

                for (int i = 0; i < samplesCount; i += batchSize)
                {
                    var batch = samples.Skip(i).Take(batchSize);
                    await PostBatchAsync(client, batch);
                }
            }
        }

        private async Task PostBatchAsync(HttpClient client, IEnumerable<Sample> batch)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7217/Analysis/ImportSamples");
            var content = new StringContent(JsonSerializer.Serialize(batch), Encoding.UTF8, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}

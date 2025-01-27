using API.DAL;
using API.Models.Domain;

namespace API.Services
{
    public class SampleService : ISampleService
    {
        private readonly IRepository<Sample> sampleRepository;
        private const int pageSize = 1000;

        public SampleService(IRepository<Sample> sampleRepository)
        {
            this.sampleRepository = sampleRepository;
        }

        public async Task<IEnumerable<Sample>> GetSamples(int page)
        {
            return (await sampleRepository.GetAsync(page, pageSize)).ToList();
        }

        public async Task ImportSamples(IEnumerable<Sample> samples)
        {
            await sampleRepository.CreateManyAsync(samples);
            await sampleRepository.SaveAsync();
        }
    }
}

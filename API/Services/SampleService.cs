using API.DAL;
using API.Models.Domain;
using API.Models.DTO;

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
            await sampleRepository.BulkCopyAsync(samples);
        }

        public async Task<IEnumerable<OutOfRangeMeasureDTO>> AnalyzeSamples(int startMm, int endMm, int threshold)
        {
            var samples = await sampleRepository.GetWhereAsync(s => s.Mm >= startMm && s.Mm <= endMm);
            var outOfRangeMeasures = samples.Where(s => Math.Abs(s.Parameter1) > threshold || Math.Abs(s.Parameter2) > threshold || Math.Abs(s.Parameter3) > threshold || Math.Abs(s.Parameter4) > threshold)
                .Select(s => new OutOfRangeMeasureDTO
                {
                    Mm = s.Mm,
                    Parameter1 = Math.Abs(s.Parameter1) > threshold ? s.Parameter1 : null,
                    Parameter2 = Math.Abs(s.Parameter2) > threshold ? s.Parameter2 : null,
                    Parameter3 = Math.Abs(s.Parameter3) > threshold ? s.Parameter3 : null,
                    Parameter4 = Math.Abs(s.Parameter4) > threshold ? s.Parameter4 : null
                })
                .ToList();

            return outOfRangeMeasures;
        }
    }
}

using API.Models.Domain;
using API.Models.DTO;

namespace API.Services
{
    public interface ISampleService
    {
        Task<IEnumerable<Sample>> GetSamples(int page);
        Task ImportSamples(IEnumerable<Sample> samples);
        Task<IEnumerable<OutOfRangeMeasureDTO>> AnalyzeSamples(int startMm, int endMm, int threshold);
    }
}

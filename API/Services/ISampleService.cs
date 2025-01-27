using API.Models.Domain;

namespace API.Services
{
    public interface ISampleService
    {
        Task<IEnumerable<Sample>> GetSamples(int page);
        Task ImportSamples(IEnumerable<Sample> samples);
    }
}

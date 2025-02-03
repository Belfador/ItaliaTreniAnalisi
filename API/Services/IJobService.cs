using API.Models.Domain;
using System.Collections.Concurrent;

namespace API.Services
{
    public interface IJobService
    {
        Job CreateJob();
        Job? GetJobStatus(Guid jobId);
    }
}

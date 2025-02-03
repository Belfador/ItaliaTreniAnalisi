using API.Models.Domain;
using System.Collections.Concurrent;

namespace API.Services
{
    public class JobService : IJobService
    {
        private readonly ConcurrentDictionary<Guid, Job> jobs = new();

        public Job CreateJob()
        {
            var id = Guid.NewGuid();
            var job = new Job
            {
                Id = id,
                Progress = 0,
                IsCompleted = false
            };
            jobs.TryAdd(id, job);

            return job;
        }

        public Job? GetJobStatus(Guid jobId)
        {
            jobs.TryGetValue(jobId, out var job);
            return job;
        }
    }
}
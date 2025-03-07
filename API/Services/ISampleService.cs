﻿using API.Models.Domain;
using API.Models.DTO;

namespace API.Services
{
    public interface ISampleService
    {
        Task<IEnumerable<Sample>> GetSamples(int page);
        Task ImportSamples(Job job, IEnumerable<Sample> samples);
        Task AnalyzeSamples(Job job, int startMm, int endMm, double threshold);
    }
}

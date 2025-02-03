using API.Models.Domain;
using API.Models.DTO;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Nelibur.ObjectMapper;

namespace ItaliaTreniAnalisi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AnalysisController : ControllerBase
    {
        private readonly ISampleService sampleService;
        private readonly IJobService jobService;

        public AnalysisController(ISampleService sampleService, IJobService jobService)
        {
            this.sampleService = sampleService;
            this.jobService = jobService;
        }

        [HttpGet("getSamples/{page}")]
        public async Task<IActionResult> GetSamples([FromRoute] int page)
        {
            try
            {
                var samples = await sampleService.GetSamples(page);
                var response = TinyMapper.Map<List<Sample>, List<SampleDTO>>(samples.ToList());

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("importSamples")]
        public async Task<IActionResult> ImportSamples([FromBody] List<ImportSampleDTO> request)
        {
            try
            {
                var samples = TinyMapper.Map<List<ImportSampleDTO>, List<Sample>>(request.ToList());

                var job = jobService.CreateJob();
                await sampleService.ImportSamples(job, samples);

                var response = TinyMapper.Map<List<Sample>, List<SampleDTO>>(samples.ToList());

                return Accepted(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("analyzeSamples")]
        public async Task<IActionResult> AnalyzeSamples(int startMm, int endMm, int threshold)
        {
            try
            {
                var job = jobService.CreateJob();
                var outOfRangeMeasures = await sampleService.AnalyzeSamples(job, startMm, endMm, threshold);

                return Accepted(outOfRangeMeasures);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("status/{jobId:guid}")]
        public IActionResult GetStatus(Guid jobId)
        {
            try
            {
                var job = jobService.GetJobStatus(jobId);

                if (job is null)
                {
                    return NotFound();
                }

                return Ok(job);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

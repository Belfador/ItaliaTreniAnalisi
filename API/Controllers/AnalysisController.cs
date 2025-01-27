using API.Models.Domain;
using API.Models.DTO;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Nelibur.ObjectMapper;

namespace ItaliaTreniAnalisi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AnalysisController : ControllerBase
    {
        private readonly ISampleService sampleService;

        public AnalysisController(ISampleService sampleService)
        {
            this.sampleService = sampleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSamples(int page)
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

        [HttpPost]
        public async Task<IActionResult> ImportSamples([FromBody] List<ImportSampleDTO> request)
        {
            try
            {
                var samples = TinyMapper.Map<List<ImportSampleDTO>, List<Sample>>(request.ToList());

                await sampleService.ImportSamples(samples);
                var response = TinyMapper.Map<List<Sample>, List<SampleDTO>>(samples.ToList());

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

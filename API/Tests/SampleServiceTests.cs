using API.DAL;
using API.Models.Domain;
using API.Models.DTO;
using API.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace API.Tests.Services
{
    public class SampleServiceTests
    {
        private readonly Mock<IRepository<Sample>> sampleRepositoryMock;
        private readonly Mock<IConfiguration> configurationMock;
        private readonly SampleService sampleService;

        public SampleServiceTests()
        {
            sampleRepositoryMock = new Mock<IRepository<Sample>>();
            configurationMock = new Mock<IConfiguration>();

            configurationMock.Setup(c => c.GetValue<int>("MaxSamples")).Returns(100);
            configurationMock.Setup(c => c.GetValue<int>("SamplesPerPage")).Returns(10);

            sampleService = new SampleService(sampleRepositoryMock.Object, configurationMock.Object);
        }

        [Fact]
        public async Task GetSamples_ShouldReturnSamples()
        {
            var samples = new List<Sample> { new Sample { Mm = 1, Parameter1 = 1, Parameter2 = 2, Parameter3 = 3, Parameter4 = 4 } };
            sampleRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(samples);

            var result = await sampleService.GetSamples(1);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(samples, result);
        }

        [Fact]
        public async Task ImportSamples_ShouldThrowException_WhenTooManySamples()
        {
            var samples = Enumerable.Range(1, 101).Select(i => new Sample { Mm = i, Parameter1 = 1, Parameter2 = 2, Parameter3 = 3, Parameter4 = 4 });

            await Assert.ThrowsAsync<Exception>(() => sampleService.ImportSamples(new Job(), samples));
        }

        [Fact]
        public async Task ImportSamples_ShouldCallBulkCopyAsync()
        {
            var samples = Enumerable.Range(1, 50).Select(i => new Sample { Mm = i, Parameter1 = 1, Parameter2 = 2, Parameter3 = 3, Parameter4 = 4 });
            var job = new Job();

            await sampleService.ImportSamples(job, samples);

            sampleRepositoryMock.Verify(repo => repo.BulkCopyAsync(job, samples), Times.Once);
        }

        [Fact]
        public async Task AnalyzeSamples_ShouldSetJobResult()
        {
            var samples = new List<Sample>
            {
                new Sample { Mm = 1, Parameter1 = 1, Parameter2 = 2, Parameter3 = 3, Parameter4 = 4 },
                new Sample { Mm = 2, Parameter1 = 5, Parameter2 = 6, Parameter3 = 7, Parameter4 = 8 }
            };
            sampleRepositoryMock.Setup(repo => repo.GetWhereAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Sample, bool>>>())).ReturnsAsync(samples);
            var job = new Job();

            await sampleService.AnalyzeSamples(job, 1, 2, 4);

            Assert.True(job.IsCompleted);
            Assert.NotNull(job.Result);
            var result = job.Result as IEnumerable<OutOfRangeMeasureDTO>;
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(2, result.First().Mm);
        }
    }
}

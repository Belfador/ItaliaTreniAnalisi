using API.DAL;
using API.Models.Domain;
using API.Services;
using Moq;
using Xunit;

namespace API.Tests
{
    public class SampleServiceTests
    {
        private readonly Mock<IRepository<Sample>> mockSampleRepository;
        private readonly SampleService sampleService;

        public SampleServiceTests()
        {
            mockSampleRepository = new Mock<IRepository<Sample>>();
            sampleService = new SampleService(mockSampleRepository.Object);
        }

        [Fact]
        public async Task GetSamples_ReturnsSamples()
        {
            var samples = new List<Sample> { new Sample { Mm = 1, Parameter1 = 1, Parameter2 = 2, Parameter3 = 3, Parameter4 = 4 } };
            mockSampleRepository.Setup(repo => repo.GetAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(samples);

            var result = await sampleService.GetSamples(1);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(samples, result);
        }

        [Fact]
        public async Task ImportSamples_CallsBulkCopyAsync()
        {
            var samples = new List<Sample> { new Sample { Mm = 1, Parameter1 = 1, Parameter2 = 2, Parameter3 = 3, Parameter4 = 4 } };

            await sampleService.ImportSamples(samples);

            mockSampleRepository.Verify(repo => repo.BulkCopyAsync(samples), Times.Once);
        }

        [Fact]
        public async Task AnalyzeSamples_ReturnsOutOfRangeMeasures()
        {
            var samples = new List<Sample>
            {
                new Sample { Mm = 1, Parameter1 = 10, Parameter2 = 2, Parameter3 = 3, Parameter4 = 4 },
                new Sample { Mm = 2, Parameter1 = 1, Parameter2 = 20, Parameter3 = 3, Parameter4 = 4 },
                new Sample { Mm = 3, Parameter1 = 1, Parameter2 = 2, Parameter3 = 30, Parameter4 = 4 },
                new Sample { Mm = 4, Parameter1 = 1, Parameter2 = 2, Parameter3 = 3, Parameter4 = 40 }
            };
            mockSampleRepository.Setup(repo => repo.GetWhereAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Sample, bool>>>())).ReturnsAsync(samples);

            var result = await sampleService.AnalyzeSamples(1, 4, 5);

            Assert.NotNull(result);
            Assert.Equal(4, result.Count());
            Assert.Contains(result, r => r.Parameter1 == 10);
            Assert.Contains(result, r => r.Parameter2 == 20);
            Assert.Contains(result, r => r.Parameter3 == 30);
            Assert.Contains(result, r => r.Parameter4 == 40);
        }
    }
}

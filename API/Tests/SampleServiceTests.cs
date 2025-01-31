using API.DAL;
using API.Models.Domain;
using API.Services;
using Moq;
using Xunit;

public class SampleServiceTests
{
    private readonly Mock<IRepository<Sample>> mockSampleRepository;
    private readonly Mock<IConfiguration> mockConfiguration;
    private readonly SampleService sampleService;

    public SampleServiceTests()
    {
        mockSampleRepository = new Mock<IRepository<Sample>>();
        mockConfiguration = new Mock<IConfiguration>();

        mockConfiguration.Setup(c => c.GetValue<int>("MaxSamples")).Returns(100);
        mockConfiguration.Setup(c => c.GetValue<int>("SamplesPerPage")).Returns(10);

        sampleService = new SampleService(mockSampleRepository.Object, mockConfiguration.Object);
    }

    [Fact]
    public async Task GetSamples_ReturnsSamples()
    {
        var samples = new List<Sample> { new Sample { Mm = 1, Parameter1 = 1, Parameter2 = 2, Parameter3 = 3, Parameter4 = 4 } };
        mockSampleRepository.Setup(repo => repo.GetAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(samples);

        var result = await sampleService.GetSamples(1);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(1, result.First().Mm);
    }

    [Fact]
    public async Task ImportSamples_ThrowsException_WhenTooManySamples()
    {
        var samples = Enumerable.Range(1, 101).Select(i => new Sample { Mm = i, Parameter1 = 1, Parameter2 = 2, Parameter3 = 3, Parameter4 = 4 });

        await Assert.ThrowsAsync<Exception>(() => sampleService.ImportSamples(samples));
    }

    [Fact]
    public async Task ImportSamples_CallsBulkCopyAsync()
    {
        var samples = Enumerable.Range(1, 50).Select(i => new Sample { Mm = i, Parameter1 = 1, Parameter2 = 2, Parameter3 = 3, Parameter4 = 4 });

        await sampleService.ImportSamples(samples);

        mockSampleRepository.Verify(repo => repo.BulkCopyAsync(samples), Times.Once);
    }

    [Fact]
    public async Task AnalyzeSamples_ReturnsOutOfRangeMeasures()
    {
        var samples = new List<Sample>
        {
            new Sample { Mm = 1, Parameter1 = 10, Parameter2 = 2, Parameter3 = 3, Parameter4 = 4 },
            new Sample { Mm = 2, Parameter1 = 1, Parameter2 = 20, Parameter3 = 3, Parameter4 = 4 }
        };
        mockSampleRepository.Setup(repo => repo.GetWhereAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Sample, bool>>>())).ReturnsAsync(samples);

        var result = await sampleService.AnalyzeSamples(0, 10, 5);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal(10, result.First().Parameter1);
        Assert.Null(result.First().Parameter2);
    }
}

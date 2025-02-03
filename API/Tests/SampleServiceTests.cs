using API.DAL;
using API.Models.Domain;
using API.Services;
using Moq;
using Xunit;

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
    public async Task GetSamples_ReturnsSamples()
    {
        // Arrange
        var samples = new List<Sample> { new Sample { Mm = 1, Parameter1 = 1, Parameter2 = 2, Parameter3 = 3, Parameter4 = 4 } };
        sampleRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(samples);

        // Act
        var result = await sampleService.GetSamples(1);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(samples, result);
    }

    [Fact]
    public async Task ImportSamples_ThrowsException_WhenTooManySamples()
    {
        // Arrange
        var samples = Enumerable.Range(1, 101).Select(i => new Sample { Mm = i, Parameter1 = 1, Parameter2 = 2, Parameter3 = 3, Parameter4 = 4 });

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => sampleService.ImportSamples(new Job(), samples));
    }

    [Fact]
    public async Task ImportSamples_CallsBulkCopyAsync()
    {
        // Arrange
        var samples = Enumerable.Range(1, 50).Select(i => new Sample { Mm = i, Parameter1 = 1, Parameter2 = 2, Parameter3 = 3, Parameter4 = 4 });
        var job = new Job();

        // Act
        await sampleService.ImportSamples(job, samples);

        // Assert
        sampleRepositoryMock.Verify(repo => repo.BulkCopyAsync(job, samples), Times.Once);
    }

    [Fact]
    public async Task AnalyzeSamples_ReturnsOutOfRangeMeasures()
    {
        // Arrange
        var samples = new List<Sample>
        {
            new Sample { Mm = 1, Parameter1 = 10, Parameter2 = 2, Parameter3 = 3, Parameter4 = 4 },
            new Sample { Mm = 2, Parameter1 = 1, Parameter2 = 20, Parameter3 = 3, Parameter4 = 4 }
        };
        sampleRepositoryMock.Setup(repo => repo.GetWhereAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Sample, bool>>>())).ReturnsAsync(samples);
        var job = new Job();

        // Act
        var result = await sampleService.AnalyzeSamples(job, 0, 10, 5);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, r => r.Parameter1 == 10);
        Assert.Contains(result, r => r.Parameter2 == 20);
    }
}

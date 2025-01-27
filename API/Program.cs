using API.DAL;
using API.Models.Domain;
using API.Models.DTO;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Nelibur.ObjectMapper;

TinyMapper.Bind<List<Sample>, List<SampleDTO>>();
TinyMapper.Bind<List<ImportSampleDTO>, List<Sample>>();
TinyMapper.Bind<List<Sample>, List<SampleDTO>>();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("LocalConnection"));
});

builder.Services.AddScoped<IRepository<Sample>, RepositoryBase<Sample>>();
builder.Services.AddScoped<ISampleService, SampleService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

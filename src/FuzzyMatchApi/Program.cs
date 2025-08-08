using FuzzyMatchApi.Core.Extensions;
using FuzzyMatchApi.Infrastructure.Extensions;
using FuzzyMatchApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add Services from each layer
builder.Services.AddApiServices();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();


var app = builder.Build();

app.UseHttpsRedirection();

app.Run();
using DisasterPrediction.Api.Extensions;
using DisasterPrediction.Api.Handlers;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.ConfigureAppSettings(builder);
builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
builder.Services.ConfigureHttpClient(builder.Configuration);
builder.Services.ConfigureServices();
builder.Services.ConfigureRedisCache(builder.Configuration);
builder.Services.ConfigureDbContext(builder.Configuration);
builder.Services.AddControllers(options => options.Filters.Add<ValidationFilterAttribute>());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

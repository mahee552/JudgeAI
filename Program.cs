// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

using ChatbotBenchmarkAPI.Infrastructure.Extenstions;
using ChatbotBenchmarkAPI.Models.Configurations.Endpoints;
using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Load additional JSON files
builder.Configuration.AddJsonFile("aiEndpoints.json", optional: true, reloadOnChange: true);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddFastEndpoints();

builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "JudgeAI API";
        s.Version = "v1";
    };
});

builder.Services.Configure<AIEndpointsConfig>(builder.Configuration.GetSection("AIEndpoints"));

builder.Services.AddAIServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsProduction())
{
    app.UseSwaggerGen();
}

app.UseFastEndpoints(c => c.Endpoints.RoutePrefix = "api");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

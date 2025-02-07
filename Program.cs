// Copyright (c) Happy Solutions.
// All rights reserved.
// This code is proprietary and confidential.
// Unauthorized copying of this file, via any medium, is strictly prohibited.

using ChatbotBenchmarkAPI.Infrastructure.Services.Factory;
using ChatbotBenchmarkAPI.Infrastructure.Services.Interfaces;
using ChatbotBenchmarkAPI.Infrastructure.Services.Providers;
using ChatbotBenchmarkAPI.Models.Configurations;
using FastEndpoints;

var builder = WebApplication.CreateBuilder(args);

// Load additional JSON files
builder.Configuration.AddJsonFile("aiEndpoints.json", optional: true, reloadOnChange: true);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddFastEndpoints();

builder.Services.Configure<AIEndpointsConfig>(builder.Configuration.GetSection("AIEndpoints"));

builder.Services.AddTransient<OpenAIService>();

builder.Services.AddTransient<DeepSeekService>();

builder.Services.AddTransient<AnthropicService>();

builder.Services.AddSingleton<IAIProviderFactory, AIProviderFactory>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseFastEndpoints();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

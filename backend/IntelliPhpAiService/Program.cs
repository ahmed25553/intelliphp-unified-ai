using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IntelliPhpAiService.Models;
using IntelliPhpAiService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<OnnxModelService>();
builder.Services.AddSingleton<TokenizationService>();
builder.Services.AddSingleton<InferenceEngine>();

var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health", () => Results.Ok(new { status = "ok", timestamp = DateTime.UtcNow }));

app.MapPost("/api/chat", async (ChatRequest request, InferenceEngine engine) =>
{
    var response = await engine.InferAsync(request);
    return Results.Ok(response);
});

app.MapPost("/api/code", async (CodeRequest request, InferenceEngine engine) =>
{
    var response = await engine.CompleteCodeAsync(request);
    return Results.Ok(response);
});

app.MapPost("/api/data", async (DataRequest request, InferenceEngine engine) =>
{
    var response = await engine.AnalyzeDataAsync(request);
    return Results.Ok(response);
});

app.Run("http://localhost:5050");

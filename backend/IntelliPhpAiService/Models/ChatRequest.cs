using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IntelliPhpAiService.Models;
using IntelliPhpAiService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<OnnxChatService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/api/chat", async (ChatRequest request, OnnxChatService service) =>
{
    var reply = await service.GenerateAsync(request.Prompt, request.Mode);
    return Results.Ok(new { reply });
});

app.Run();

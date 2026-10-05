using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using IntelliPhpAiService.Models;

namespace IntelliPhpAiService.Services;

public sealed class InferenceEngine
{
    private readonly OnnxModelService _modelService;
    private readonly TokenizationService _tokenizer;

    public InferenceEngine(OnnxModelService modelService, TokenizationService tokenizer)
    {
        _modelService = modelService;
        _tokenizer = tokenizer;
    }

    public async Task<InferenceResponse> InferAsync(ChatRequest request)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
            {
                return new InferenceResponse
                {
                    Output = "Prompt cannot be empty.",
                    Mode = "chat",
                    Success = false,
                    ProcessingTimeMs = stopwatch.ElapsedMilliseconds
                };
            }

            var (inputIds, attentionMask) = await _tokenizer.EncodeAsync(request.Prompt);

            var result = await _modelService.RunInferenceAsync();

            var output = GenerateChatResponse(request.Prompt, request.Temperature);

            stopwatch.Stop();

            return new InferenceResponse
            {
                Output = output,
                Mode = "chat",
                Success = true,
                ProcessingTimeMs = stopwatch.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return new InferenceResponse
            {
                Output = string.Empty,
                Mode = "chat",
                Success = false,
                Error = ex.Message,
                ProcessingTimeMs = stopwatch.ElapsedMilliseconds
            };
        }
    }

    public async Task<InferenceResponse> CompleteCodeAsync(CodeRequest request)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var (inputIds, _) = await _tokenizer.EncodeAsync(request.Code);

            var completion = GenerateCodeCompletion(request.Code, request.Language);

            stopwatch.Stop();

            return new InferenceResponse
            {
                Output = completion,
                Mode = "code",
                Success = true,
                ProcessingTimeMs = stopwatch.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return new InferenceResponse
            {
                Output = string.Empty,
                Mode = "code",
                Success = false,
                Error = ex.Message,
                ProcessingTimeMs = stopwatch.ElapsedMilliseconds
            };
        }
    }

    public async Task<InferenceResponse> AnalyzeDataAsync(DataRequest request)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var (inputIds, _) = await _tokenizer.EncodeAsync(request.Input);

            var analysis = GenerateDataAnalysis(request.Input, request.Format);

            stopwatch.Stop();

            return new InferenceResponse
            {
                Output = analysis,
                Mode = "data",
                Success = true,
                ProcessingTimeMs = stopwatch.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return new InferenceResponse
            {
                Output = string.Empty,
                Mode = "data",
                Success = false,
                Error = ex.Message,
                ProcessingTimeMs = stopwatch.ElapsedMilliseconds
            };
        }
    }

    private string GenerateChatResponse(string prompt, float temperature)
    {
        var sb = new StringBuilder();
        sb.AppendLine("🤖 Chat Response:");
        sb.AppendLine($"Temperature: {temperature}");
        sb.AppendLine($"Input: {prompt}");
        sb.AppendLine();
        sb.AppendLine("⏳ This is a placeholder response.");
        sb.AppendLine("Connect your ONNX model to get real inference results.");
        return sb.ToString();
    }

    private string GenerateCodeCompletion(string code, string language)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"💻 Code Completion ({language}):");
        sb.AppendLine("---");
        sb.AppendLine(code);
        sb.AppendLine("// [Completion placeholder - connect ONNX model]");
        return sb.ToString();
    }

    private string GenerateDataAnalysis(string input, string format)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"📊 Data Analysis ({format}):");
        sb.AppendLine("---");
        sb.AppendLine(input);
        sb.AppendLine();
        sb.AppendLine("// [Analysis placeholder - connect ONNX model]");
        return sb.ToString();
    }
}

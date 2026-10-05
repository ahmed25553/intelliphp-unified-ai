using System;
using System.IO;
using System.Threading.Tasks;

namespace IntelliPhpAiService.Services;

public sealed class OnnxChatService
{
    private readonly string _modelPath;

    public OnnxChatService()
    {
        var baseDir = AppContext.BaseDirectory;
        _modelPath = Path.Combine(baseDir, "..", "..", "..", "..", "..", "models", "chat-code-data", "model.onnx");
        _modelPath = Path.GetFullPath(_modelPath);
    }

    public Task<string> GenerateAsync(string prompt, string mode = "chat")
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            return Task.FromResult("Prompt cannot be empty.");
        }

        if (!File.Exists(_modelPath))
        {
            return Task.FromResult($"Model file not found: {_modelPath}. Add your ONNX model to the models/chat-code-data folder.");
        }

        return Task.FromResult($"[{mode}] Local inference ready. Model path: {_modelPath}.\n\nPrompt: {prompt}\n\nThis is the placeholder response while the real model pipeline is connected.");
    }
}

namespace IntelliPhpAiService.Models
{
    public class ChatRequest
    {
        public string Prompt { get; set; } = string.Empty;
        public string Mode { get; set; } = "chat";
    }
}

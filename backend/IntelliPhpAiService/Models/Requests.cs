namespace IntelliPhpAiService.Models;

public class ChatRequest
{
    public string Prompt { get; set; } = string.Empty;
    public string Mode { get; set; } = "chat";
    public int MaxTokens { get; set; } = 256;
    public float Temperature { get; set; } = 0.7f;
}

public class CodeRequest
{
    public string Code { get; set; } = string.Empty;
    public string Language { get; set; } = "csharp";
    public int MaxTokens { get; set; } = 512;
    public bool Complete { get; set; } = true;
}

public class DataRequest
{
    public string Input { get; set; } = string.Empty;
    public string Format { get; set; } = "json";
    public int MaxTokens { get; set; } = 256;
}

public class InferenceResponse
{
    public string Output { get; set; } = string.Empty;
    public string Mode { get; set; } = string.Empty;
    public long ProcessingTimeMs { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
}

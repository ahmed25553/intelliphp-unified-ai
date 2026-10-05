using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace IntelliPhpAiService.Services;

public sealed class OnnxModelService : IDisposable
{
    private InferenceSession? _session;
    private readonly string _modelPath;
    private bool _isInitialized = false;

    public OnnxModelService()
    {
        // Try multiple paths in order:
        // 1. Environment variable
        var envPath = Environment.GetEnvironmentVariable("INTELLIPHP_MODEL_PATH");
        if (!string.IsNullOrEmpty(envPath) && File.Exists(envPath))
        {
            _modelPath = envPath;
        }
        // 2. Default local path (your machine)
        else if (File.Exists(@"D:\models\code-llm-model\intelliphp_v3\model.onnx"))
        {
            _modelPath = @"D:\models\code-llm-model\intelliphp_v3\model.onnx";
        }
        // 3. Project relative path
        else
        {
            var baseDir = AppContext.BaseDirectory;
            _modelPath = Path.Combine(
                baseDir, "..", "..", "..", "..", "..",
                "models", "chat-code-data", "model.onnx"
            );
            _modelPath = Path.GetFullPath(_modelPath);
        }
    }

    public bool Initialize()
    {
        try
        {
            Console.WriteLine($"🔍 Looking for model at: {_modelPath}");

            if (!File.Exists(_modelPath))
            {
                Console.WriteLine($"❌ Model not found at: {_modelPath}");
                Console.WriteLine($"📋 Attempted paths:");
                Console.WriteLine($"   1. Environment variable: INTELLIPHP_MODEL_PATH");
                Console.WriteLine($"   2. Default: D:\\models\\code-llm-model\\intelliphp_v3\\model.onnx");
                Console.WriteLine($"   3. Project: models/chat-code-data/model.onnx");
                return false;
            }

            var fileInfo = new FileInfo(_modelPath);
            Console.WriteLine($"✅ Model found: {fileInfo.Name} ({FormatBytes(fileInfo.Length)})");

            var sessionOptions = new SessionOptions
            {
                LogSeverityLevel = OrtLoggingLevel.ORT_LOGGING_LEVEL_WARNING,
                GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL
            };

            _session = new InferenceSession(_modelPath, sessionOptions);
            _isInitialized = true;

            PrintModelInfo();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to initialize ONNX model: {ex.Message}");
            return false;
        }
    }

    private void PrintModelInfo()
    {
        if (_session == null) return;

        Console.WriteLine("\n📊 Model Information:");
        Console.WriteLine($"   ✓ Inputs ({_session.InputNames.Count}):");
        foreach (var input in _session.InputNames)
        {
            Console.WriteLine($"     - {input}");
        }

        Console.WriteLine($"   ✓ Outputs ({_session.OutputNames.Count}):");
        foreach (var output in _session.OutputNames)
        {
            Console.WriteLine($"     - {output}");
        }
        Console.WriteLine();
    }

    public async Task<Dictionary<string, object>> RunInferenceAsync(
        Dictionary<string, Tensor<int64>>? inputIds = null,
        int batchSize = 1,
        int seqLen = 128)
    {
        if (!_isInitialized || _session == null)
        {
            if (!Initialize())
            {
                return new Dictionary<string, object>
                {
                    { "error", "Model initialization failed. Check console output for details." },
                    { "status", "error" }
                };
            }
        }

        if (_session == null)
        {
            return new Dictionary<string, object>
            {
                { "error", "Model session is null" },
                { "status", "error" }
            };
        }

        await Task.Delay(10);

        try
        {
            // Return model metadata for now
            return new Dictionary<string, object>
            {
                { "status", "ready" },
                { "modelInputs", _session.InputNames.ToList() },
                { "modelOutputs", _session.OutputNames.ToList() },
                { "message", "✅ ONNX model loaded and ready for inference" }
            };
        }
        catch (Exception ex)
        {
            return new Dictionary<string, object>
            {
                { "error", ex.Message },
                { "status", "error" }
            };
        }
    }

    private string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }

    public void Dispose()
    {
        _session?.Dispose();
    }
}

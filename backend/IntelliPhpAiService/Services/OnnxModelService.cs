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
        var baseDir = AppContext.BaseDirectory;
        _modelPath = Path.Combine(
            baseDir, "..", "..", "..", "..", "..",
            "models", "chat-code-data", "model.onnx"
        );
        _modelPath = Path.GetFullPath(_modelPath);
    }

    public bool Initialize()
    {
        try
        {
            if (!File.Exists(_modelPath))
            {
                Console.WriteLine($"⚠️  Model not found at: {_modelPath}");
                return false;
            }

            var sessionOptions = new SessionOptions
            {
                LogSeverityLevel = OrtLoggingLevel.ORT_LOGGING_LEVEL_WARNING
            };

            _session = new InferenceSession(_modelPath, sessionOptions);
            _isInitialized = true;

            Console.WriteLine($"✅ Model loaded: {_modelPath}");
            Console.WriteLine($"   Inputs: {_session.InputNames.Count}");
            Console.WriteLine($"   Outputs: {_session.OutputNames.Count}");

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to initialize ONNX model: {ex.Message}");
            return false;
        }
    }

    public async Task<Dictionary<string, object>> RunInferenceAsync(
        Dictionary<string, Tensor<int64>>? inputIds = null,
        int batchSize = 1,
        int seqLen = 128)
    {
        if (!_isInitialized || _session == null)
        {
            Initialize();
        }

        if (_session == null)
        {
            return new Dictionary<string, object>
            {
                { "error", "Model not initialized. Ensure model.onnx exists in models/chat-code-data/" }
            };
        }

        await Task.Delay(50);

        return new Dictionary<string, object>
        {
            { "status", "placeholder" },
            { "message", "ONNX inference ready. Connect your actual model data." }
        };
    }

    public void Dispose()
    {
        _session?.Dispose();
    }
}

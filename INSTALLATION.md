# 🚀 IntelliPhp Unified AI - Installation Guide

## Prerequisites

- **.NET 8.0 SDK** - [Download](https://dotnet.microsoft.com/download)
- **Node.js 18+** - [Download](https://nodejs.org/)
- **VS Code 1.85+** - [Download](https://code.visualstudio.com/)

## Step 1: Clone & Setup Backend

```bash
cd backend/IntelliPhpAiService
dotnet restore
dotnet run
```

✅ Backend will start on `http://localhost:5050`

## Step 2: Build Extension

```bash
cd extension
npm install
npm run compile
```

## Step 3: Add Model Binary

**Place your `model.onnx` file here:**
```
models/chat-code-data/
├── model.onnx          ← Your model binary (required)
├── model.json          ✅ Already here
├── metadata.json       ✅ Already here
└── genai_config.json   ✅ Already here
```

## Step 4: Run Extension

Open `extension` folder in VS Code and press **F5**

## Commands

| Command | Shortcut | Function |
|---------|----------|----------|
| Chat | `Ctrl+Shift+Alt+C` | Ask questions |
| Complete Code | `Ctrl+Shift+Alt+K` | Generate code |
| Analyze Data | Command Palette | Process data |

## Troubleshooting

### Backend won't start
```bash
dotnet --version  # Check if .NET 8.0 installed
netstat -an | grep 5050  # Check if port is free
```

### Extension won't connect
- Verify backend is running: `curl http://localhost:5050/health`
- Check VS Code Output panel (View → Output)

### Model not loading
- Ensure `model.onnx` exists in `models/chat-code-data/`
- Check backend logs for file path error

## Model Configuration

**Your Model Specs:**
- Type: GPT-2 Quantized (ONNX)
- Vocab Size: 32,000
- Context Length: 512
- Layers: 8
- Heads: 8
- Embedding Dim: 512
- Inference: Beam Search + Past State Management

## Next Steps

1. ✅ Configuration added
2. 📦 Place model binary
3. 🔧 Run backend
4. 🎮 Launch extension
5. 💬 Start chatting!

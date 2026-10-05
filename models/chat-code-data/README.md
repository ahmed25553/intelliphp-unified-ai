# IntelliPhp Unified AI

This repo is meant to support a lightweight local AI stack for chat, code, and data workflows in VS Code.

## Core idea

- Front-end: VS Code extension
- Backend: local .NET service
- Runtime: ONNX + Microsoft.ML + tokenizers
- Future: vision model sidecar

## Planned modules

- Chat inference
- Code generation
- Data reasoning
- Future image model

## Work in progress

The repo starts with a minimal working starter that includes:
- a VS Code extension shell
- a .NET API backend
- ONNX integration placeholder
- ready path for model loading

## Source directories

- `extension/` - VS Code extension
- `backend/` - .NET Core service
- `models/` - local model assets

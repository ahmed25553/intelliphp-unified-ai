import * as vscode from 'vscode';

const API_URL = 'http://localhost:5050';

interface ApiResponse {
  output?: string;
  success?: boolean;
  error?: string;
  processingTimeMs?: number;
}

export function activate(context: vscode.ExtensionContext) {
  console.log('✅ IntelliPhp AI Extension activated');

  const chatCommand = vscode.commands.registerCommand('intelliphp-ai.chat', async () => {
    const input = await vscode.window.showInputBox({
      prompt: '💬 Chat with IntelliPhp AI',
      placeHolder: 'Ask anything...'
    });

    if (!input) return;
    await callAI('chat', { prompt: input, mode: 'chat', temperature: 0.7 });
  });

  const codeCommand = vscode.commands.registerCommand('intelliphp-ai.complete-code', async () => {
    const editor = vscode.window.activeTextEditor;
    if (!editor) {
      vscode.window.showErrorMessage('❌ No active editor');
      return;
    }

    const selectedText = editor.document.getText(editor.selection);
    const code = selectedText || editor.document.getText();
    const language = editor.document.languageId;

    await callAI('code', { code, language, complete: true, maxTokens: 512 });
  });

  const dataCommand = vscode.commands.registerCommand('intelliphp-ai.analyze-data', async () => {
    const input = await vscode.window.showInputBox({
      prompt: '📊 Paste data to analyze',
      placeHolder: 'JSON, CSV, or text...'
    });

    if (!input) return;
    await callAI('data', { input, format: 'json', maxTokens: 256 });
  });

  context.subscriptions.push(chatCommand, codeCommand, dataCommand);
}

async function callAI(type: string, payload: any) {
  const panel = vscode.window.createWebviewPanel(
    'intelliphpAi',
    `🤔 IntelliPhp - Processing...`,
    vscode.ViewColumn.One,
    {}
  );

  panel.webview.html = getLoadingHtml();

  try {
    const endpoint = `/api/${type}`;
    const response = await fetch(API_URL + endpoint, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload)
    });

    if (!response.ok) {
      throw new Error(`HTTP ${response.status}`);
    }

    const data = (await response.json()) as ApiResponse;

    if (data.success) {
      panel.title = `✅ IntelliPhp - ${type.toUpperCase()}`;
      panel.webview.html = getSuccessHtml(data.output || '', data.processingTimeMs || 0);
    } else {
      panel.title = `❌ IntelliPhp - Error`;
      panel.webview.html = getErrorHtml(data.error || 'Unknown error');
    }
  } catch (error) {
    panel.title = `❌ Connection Error`;
    const message = error instanceof Error ? error.message : 'Unknown error';
    panel.webview.html = getErrorHtml(
      `${message}\n\nMake sure backend is running:\ndotnet run (in backend folder)`
    );
  }
}

function getLoadingHtml(): string {
  return `
    <html>
      <head>
        <style>
          body { font-family: 'Courier New', monospace; padding: 30px; background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%); color: #e2e8f0; }
          .spinner { display: inline-block; width: 30px; height: 30px; border: 4px solid #60a5fa; border-top: 4px solid transparent; border-radius: 50%; animation: spin 1s linear infinite; }
          @keyframes spin { to { transform: rotate(360deg); } }
          h2 { color: #60a5fa; margin: 0 0 15px 0; }
        </style>
      </head>
      <body>
        <h2>🤔 Thinking...</h2>
        <div class="spinner"></div>
        <p>Contacting IntelliPhp backend...</p>
      </body>
    </html>
  `;
}

function getSuccessHtml(output: string, timeMs: number): string {
  return `
    <html>
      <head>
        <style>
          body { font-family: 'Courier New', monospace; padding: 20px; background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%); color: #e2e8f0; }
          h3 { color: #10b981; margin-top: 0; }
          pre { background: #1e293b; padding: 15px; border-radius: 6px; overflow-x: auto; border-left: 4px solid #10b981; }
          .meta { color: #94a3b8; font-size: 12px; margin-top: 10px; }
        </style>
      </head>
      <body>
        <h3>✅ Response</h3>
        <pre>${escapeHtml(output)}</pre>
        <div class="meta">⏱️ Processing time: ${timeMs}ms</div>
      </body>
    </html>
  `;
}

function getErrorHtml(error: string): string {
  return `
    <html>
      <head>
        <style>
          body { font-family: 'Courier New', monospace; padding: 20px; background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%); color: #e2e8f0; }
          h3 { color: #ef4444; margin-top: 0; }
          pre { background: #1f2937; padding: 15px; border-radius: 6px; border-left: 4px solid #ef4444; color: #fca5a5; }
        </style>
      </head>
      <body>
        <h3>❌ Error</h3>
        <pre>${escapeHtml(error)}</pre>
      </body>
    </html>
  `;
}

function escapeHtml(text: string): string {
  const map: { [key: string]: string } = {
    '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;'
  };
  return text.replace(/[&<>"']/g, (char) => map[char]);
}

export function deactivate() {}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML.Tokenizers;

namespace IntelliPhpAiService.Services;

public sealed class TokenizationService
{
    private Tokenizer? _tokenizer;
    private bool _initialized = false;

    public TokenizationService()
    {
        Initialize();
    }

    private void Initialize()
    {
        try
        {
            _tokenizer = new Bpe();
            _initialized = true;
            Console.WriteLine("✅ Tokenizer initialized (BPE fallback)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️  Tokenizer init warning: {ex.Message}");
            _initialized = false;
        }
    }

    public async Task<(long[] InputIds, long[] AttentionMask)> EncodeAsync(string text, int maxLength = 512)
    {
        await Task.Delay(10);

        if (string.IsNullOrWhiteSpace(text))
        {
            return (new long[] { }, new long[] { });
        }

        var tokens = new List<long>();
        var words = text.Split(new[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var word in words)
        {
            var tokenIds = word
                .ToCharArray()
                .Select(c => (long)c)
                .ToList();
            tokens.AddRange(tokenIds);

            if (tokens.Count >= maxLength)
            {
                break;
            }
        }

        var inputIds = tokens.Take(maxLength).ToArray();
        var attentionMask = new long[inputIds.Length];
        Array.Fill(attentionMask, 1L);

        return (inputIds, attentionMask);
    }

    public async Task<string> DecodeAsync(long[] tokenIds)
    {
        await Task.Delay(10);

        if (tokenIds == null || tokenIds.Length == 0)
        {
            return string.Empty;
        }

        var chars = tokenIds
            .Where(id => id >= 32 && id < 127)
            .Select(id => (char)id)
            .ToArray();

        return new string(chars);
    }
}

using Inventai.Core;
using OpenAI.Chat;
using System.ClientModel;

namespace Inventai.TextAgents;

/// <summary>
/// List of available OpenAI text models
/// </summary>
public static class EOpenAITextModels
{
    public static readonly string? None = null;
    public static readonly string GPT35turbo = "gpt-3.5-turbo";
    public static readonly string GPT4o = "gpt-4o";
    public static readonly string GPT4turbo = "gpt-4-turbo";
    public static readonly string GPT4omini = "gpt-4o-mini";
}

/// <summary>
/// OpenAI Text Agent
/// </summary>
public class TextAgentOpenAI : ITextAgent
{
    private readonly ChatClient _chatClient;

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="pModel"></param>
    /// <param name="pApiKey"></param>
    /// <exception cref="ChatClientException"></exception>
    public TextAgentOpenAI(string pModel, string pApiKey)
    {
        try
        {
            _chatClient = new ChatClient(pModel, new ApiKeyCredential(pApiKey));
            Console.WriteLine($"OpenAI chat client created with model {pModel}");
        }
        catch (Exception e)
        {
            throw new ChatClientException($"Error creating the chat client. Please check your API keys - {e}");
        }
    }

    public string CompleteMessage(string pMessage)
    {
        ChatCompletion completion = _chatClient.CompleteChat(pMessage);
        return completion.Content[0].Text; // we ignore the potential attachments
    }
}
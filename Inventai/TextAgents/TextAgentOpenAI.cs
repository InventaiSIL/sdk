using Inventai.Core;
using OpenAI.Chat;

namespace Inventai.TextAgents;

/// <summary>
/// OpenAI Agent
/// </summary>
public class TextAgentOpenAI : ITextAgent
{
    private readonly ChatClient _chatClient;

    public TextAgentOpenAI(string pModel)
    {
        try
        {
            _chatClient = new(model: pModel, apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        }
        catch (Exception e)
        {
            throw new ChatClientException($"Error creating the chat client. Please check your API keys - ${e}");
        }
    }

    public string CompleteMessage(string pMessage)
    {
        ChatCompletion completion = _chatClient.CompleteChat(pMessage);

        return completion.Content[0].Text; // we ignore the potential attachments
    }
}
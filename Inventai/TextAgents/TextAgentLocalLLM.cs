using Inventai.Core;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace Inventai.TextAgents
{
    public class TextAgentLocalLLM : ITextAgent
    {
        private readonly ChatClient _chatClient;

        public TextAgentLocalLLM(string model, Uri endpoint)
        {
            try
            {
                _chatClient = new ChatClient(model, new ApiKeyCredential("dummy"), new OpenAIClientOptions { Endpoint = endpoint });
            }
            catch (Exception e)
            {
                throw new ChatClientException($"Error creating the chat client with local LLM - {e}");
            }
        }

        public string CompleteMessage(string message)
        {
            ChatCompletion completion = _chatClient.CompleteChat(message);
            return completion.Content[0].Text;
        }
    }
}


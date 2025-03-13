using Inventai.Core;
using OpenAI;
using OpenAI.Chat;
using System;
using System.ClientModel;


namespace Inventai.TextAgents
{
    public class TextAgentLocalLlm : ITextAgent
    {
        private readonly ChatClient _chatClient;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="pModel"></param>
        /// <param name="pEndpoint"></param>
        /// <exception cref="ChatClientException"></exception>
        public TextAgentLocalLlm(string pModel, Uri pEndpoint)
        {
            try
            {
                _chatClient = new ChatClient(pModel, new ApiKeyCredential("dummy"), new OpenAIClientOptions { Endpoint = pEndpoint });
            }
            catch (Exception e)
            {
                throw new ChatClientException($"Error creating the chat client with local LLM - {e}");
            }
        }

        public string CompleteMessage(string pMessage)
        {
            ChatCompletion completion = _chatClient.CompleteChat(pMessage);
            return completion.Content[0].Text;
        }
    }

}


using Inventai.Core.Discussion;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace Inventai.Src.Discussion
{
    /// <summary>
    /// Inventai's discussion context
    /// </summary>
    public class DiscussionContextManager : IDiscussionContext
    {
        private readonly Inventai.Core.ITextAgent m_TextAgent;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="pTextAgent"></param>
        public DiscussionContextManager(Core.ITextAgent pTextAgent)
        {
            m_TextAgent = pTextAgent;
            Console.WriteLine("Discussion context created");
        }

        public ContextualChoicesResponse GenerateContextualChoices(ContextualChoicesRequest pRequest)
        {
            Console.WriteLine("Generating contextual choices...");

            var prompt = new StringBuilder();
            prompt.Append("You have to generate choices for the prompt: ");
            prompt.Append(pRequest.Prompt);
            prompt.Append(" with the context: ");
            prompt.Append(pRequest.Context);
            prompt.Append(" and the number of choices: ");
            prompt.Append(pRequest.NumChoices);
            prompt.Append(".\n");
            prompt.Append("Respond with only the choices as a list of strings.");

            var responseString = m_TextAgent.CompleteMessage(prompt.ToString());

            var response = responseString.Split("\n");

            Console.WriteLine(response.Length + " choices generated");

            return new ContextualChoicesResponse() { Choices = response };
        }

        public EntitiesChatResponse GenerateEntitiesChat(EntitiesChatRequest pRequest)
        {
            Console.WriteLine("Generating entities chats...");

            var prompt = new StringBuilder();
            prompt.Append("You have to generate ");
            prompt.Append(pRequest.NumMessages);
            prompt.Append(" messages between the entities ");
            prompt.Append(pRequest.Entities.ToString());
            prompt.Append(" for the prompt: ");
            prompt.Append(pRequest.Prompt);
            prompt.Append(" with the context: ");
            prompt.Append(pRequest.Context);
            prompt.Append(".\n");
            prompt.Append("Respond with only the entities chats as JSON that is of type EntitiesDiscussion that contains the Participants and EntitiesMessage. EntitiesMessage contains an entity and a message.");

            var responseString = m_TextAgent.CompleteMessage(prompt.ToString());

            Console.WriteLine(responseString);

            // Assuming responseString is a JSON string, we need to deserialize it to List<EntitiesDiscussion>
            var response = JsonConvert.DeserializeObject<EntitiesChatResponse>(responseString);

            return response;
        }
    }
}

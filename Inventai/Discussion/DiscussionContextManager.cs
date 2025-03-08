using Inventai.Core.Discussion;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace Inventai.Discussion
{
    /// <summary>
    /// Inventai's discussion context
    /// </summary>
    public class DiscussionContextManager : IDiscussionContext
    {
        /// <summary>
        /// The text agent to use
        /// </summary>
        private readonly Core.ITextAgent m_TextAgent;

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
            prompt.Append(" messages between the entities: ");
            prompt.Append(pRequest.Entities.Select(e => e.Name).Aggregate((a, b) => a + ", " + b));
            prompt.Append(" for the prompt: ");
            prompt.Append(pRequest.Prompt);
            prompt.Append(" with the context: ");
            prompt.Append(pRequest.Context);
            prompt.Append(".\n");
            prompt.Append("Respond with only the entities chats as JSON that contains the Participants (list of participants) and EntitiesMessages " +
                "(list of object containing EntityId and a Message.");

            var responseString = m_TextAgent.CompleteMessage(prompt.ToString());

            try
            {
                var response = JsonConvert.DeserializeObject<EntitiesChatResponse>(responseString);

                return response ?? new EntitiesChatResponse() { Participants = [], EntitiesMessages = [] };
            }
            catch (JsonException e)
            {
                Debug.WriteLine(e.Message);
                return new EntitiesChatResponse() { Participants = [], EntitiesMessages = [] };
            }
        }
    }
}

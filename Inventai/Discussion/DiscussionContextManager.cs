using Inventai.Core.Discussion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            prompt.AppendLine("You are a helpful assistant that generates relevant choices based on a given context.");
            prompt.AppendLine("Task: Generate exactly " + pRequest.NumChoices + " distinct and relevant choices for the following scenario:");
            prompt.AppendLine("Prompt: " + pRequest.Prompt);
            prompt.AppendLine("Context: " + pRequest.Context);
            prompt.AppendLine("\nRequirements:");
            prompt.AppendLine("1. Each choice must be a single line of text");
            prompt.AppendLine("2. Choices should be diverse and cover different aspects of the scenario");
            prompt.AppendLine("3. Each choice should be clear and actionable");
            prompt.AppendLine("4. Do not include any numbering or bullet points");
            prompt.AppendLine("\nExample format:");
            prompt.AppendLine("Choice 1");
            prompt.AppendLine("Choice 2");
            prompt.AppendLine("Choice 3");

            var responseString = m_TextAgent.CompleteMessage(prompt.ToString());
            var response = responseString.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                      .Select(s => s.Trim())
                                      .Where(s => !string.IsNullOrWhiteSpace(s))
                                      .ToArray();

            Console.WriteLine($"{response.Length} choices generated");

            return new ContextualChoicesResponse() { Choices = response };
        }

        public EntitiesChatResponse GenerateEntitiesChat(EntitiesChatRequest pRequest)
        {
            Console.WriteLine("Generating entities chats...");

            var prompt = new StringBuilder();
            prompt.AppendLine("You are a helpful assistant that generates realistic conversations between entities.");
            prompt.AppendLine("Task: Generate a conversation with exactly " + pRequest.NumMessages + " messages between the following entities:");
            prompt.AppendLine("Entities: " + string.Join(", ", pRequest.Entities.Select(e => e.Name)));
            prompt.AppendLine("Prompt: " + pRequest.Prompt);
            prompt.AppendLine("Context: " + pRequest.Context);
            
            prompt.AppendLine("\nRequirements:");
            prompt.AppendLine("1. Generate a valid JSON response with the following structure:");
            prompt.AppendLine("   {\"Participants\": [\"Entity1\", \"Entity2\"], \"EntitiesMessages\": [{\"EntityId\": \"Entity1\", \"Message\": \"Message text\"}]}");
            prompt.AppendLine("2. Each message should be natural and relevant to the context");
            prompt.AppendLine("3. Messages should show a logical progression of the conversation");
            prompt.AppendLine("4. Each entity should participate in the conversation");
            prompt.AppendLine("5. Entity IDs must match exactly with the provided entity names");
            
            prompt.AppendLine("\nExample format:");
            prompt.AppendLine("{\"Participants\": [\"Alice\", \"Bob\"], \"EntitiesMessages\": [{\"EntityId\": \"Alice\", \"Message\": \"Hello Bob!\"}, {\"EntityId\": \"Bob\", \"Message\": \"Hi Alice!\"}]}");

            var responseString = m_TextAgent.CompleteMessage(prompt.ToString());

            try
            {
                var response = JsonConvert.DeserializeObject<EntitiesChatResponse>(responseString);
                
                // Validate the response
                if (response == null || 
                    response.Participants == null || 
                    response.EntitiesMessages == null ||
                    !response.Participants.Any() ||
                    !response.EntitiesMessages.Any())
                {
                    throw new JsonException("Invalid response structure");
                }

                // Validate that all entity IDs exist in participants
                var invalidEntityIds = response.EntitiesMessages
                    .Select(m => m.EntityId)
                    .Where(id => !response.Participants.Contains(id))
                    .ToList();

                if (invalidEntityIds.Any())
                {
                    throw new JsonException($"Invalid entity IDs found: {string.Join(", ", invalidEntityIds)}");
                }

                return response;
            }
            catch (JsonException e)
            {
                Debug.WriteLine($"Error deserializing response: {e.Message}");
                return new EntitiesChatResponse() { Participants = new List<string>(), EntitiesMessages = new List<EntityMessage>() };
            }
        }
    }
}

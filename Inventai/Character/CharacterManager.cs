using Inventai.Core;
using Inventai.Core.Character;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Inventai.Character
{
    /// <summary>
    /// Character manager
    /// </summary>
    public class CharacterManager : ICharacterManager
    {
        /// <summary>
        /// The text agent to use
        /// </summary>
        private readonly ITextAgent m_TextAgent;

        /// <summary>
        /// The image agent to use
        /// </summary>
        private readonly IImageAgent m_ImageAgent;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="pTextAgent"></param>
        public CharacterManager(ITextAgent pTextAgent, IImageAgent pImageAgent)
        {
            m_TextAgent = pTextAgent;
            m_ImageAgent = pImageAgent;
        }

        public List<Core.Character.Character> GenerateCharacters(CharacterCreationRequest pRequest)
        {
            Console.WriteLine("Generating characters for the prompt: " + pRequest.Prompt + " with context: " + pRequest.Context);
            var characters = new List<Core.Character.Character>();
            var characterNames = new List<string>();

            var prompt = $@"Create {pRequest.NumCharacters} unique and distinct characters based on the following requirements:

                Prompt: {pRequest.Prompt}
                Context: {pRequest.Context}

                For each character, provide the following information in a structured format:
                - Name: A unique and memorable name
                - MetaData: A detailed description including:
                * Physical appearance
                * Personality traits
                * Background/history
                * Special abilities or skills
                * Motivations and goals

                Format the response as a JSON array of objects with exactly these properties:
                [
                {{
                    ""Name"": ""Character Name"",
                    ""MetaData"": ""Detailed character description""
                }}
                ]

                Ensure the response is valid JSON and contains exactly {pRequest.NumCharacters} characters.";

            try
            {
                var responseString = m_TextAgent.CompleteMessage(prompt);
                var characterBases = JsonConvert.DeserializeObject<List<Core.Character.Character>>(responseString);

                if (characterBases == null || characterBases.Count != pRequest.NumCharacters)
                {
                    throw new Exception($"Failed to generate the requested number of characters. Expected {pRequest.NumCharacters}, got {characterBases?.Count ?? 0}");
                }

                for (int i = 0; i < pRequest.NumCharacters; i++)
                {
                    var character = new Core.Character.Character
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = characterBases[i].Name,
                        MetaData = characterBases[i].MetaData
                    };
                    characters.Add(character);
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error parsing character response: {ex.Message}");
                throw new Exception("Failed to parse character generation response. The response was not in the expected JSON format.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating characters: {ex.Message}");
                throw;
            }

            Console.WriteLine("Successfully generated characters: " + string.Join(", ", characters.Select(c => c.Name)));
            return characters;
        }
    }
}

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
            // Generate the characters
            var characters = new List<Core.Character.Character>();
            var characterNames = new List<string>();

            var prompt = "Create " + pRequest.NumCharacters + " characters for the prompt: " + pRequest.Prompt + " with context: " + pRequest.Context
                + " .\nEach character should have a name and some metadata describing the character."
                + " . Provide a list of the characters as JSON that contains the Name and MetaData (only the list without the key 'characters')";
            var responseString = m_TextAgent.CompleteMessage(prompt);
            var characterBases = JsonConvert.DeserializeObject<List<Core.Character.Character>>(responseString);

            for (int i = 0; i < pRequest.NumCharacters; i++)
            {
                // Generate the character
                var character = new Core.Character.Character
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = characterBases[i].Name,
                    Image = m_ImageAgent.GenerateCharacterImageAsync(
                        "Create an image for the character named " 
                        + characterBases[i].Name 
                        + " with the general context: " + pRequest.Prompt
                        + " and having the metadata: " + characterBases[i].MetaData, pRequest.Context).Result,
                    MetaData = characterBases[i].MetaData
                };
                // Add the character to the list
                characters.Add(character);
            }
            Console.WriteLine("Generated characters: " + string.Join(", ", characters.Select(c => c.Name)));
            return characters;
        }
    }
}

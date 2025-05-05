using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Inventai.Src.Entities
{
    /// <summary>
    /// Represents a character entity in the Inventai system
    /// </summary>
    public class Entities : Core.Entities.IEntities
    {
        /// <summary>
        /// Gets or sets the path to the file where character IDs are stored
        /// </summary>
        public static string PastIdPath { get; private set; }

        /// <summary>
        /// Gets the current count of characters created
        /// </summary>
        public static int CharCount { get; private set; } = 0;

        /// <summary>
        /// JSON serializer options for character serialization/deserialization
        /// </summary>
        private static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions 
        { 
            Converters = { new JsonEntitiesConverter() }, 
            PropertyNameCaseInsensitive = true, 
            WriteIndented = true 
        };

        /// <summary>
        /// Gets the unique identifier for this character
        /// </summary>
        public int CharId { get; private set; }

        /// <summary>
        /// Gets or sets the creator's note about the character
        /// </summary>
        public string CreatorNote { get; set; } = "";

        /// <summary>
        /// Gets the dictionary of character attributes
        /// </summary>
        public Dictionary<string, string> CharAttributes { get; private set; } = new Dictionary<string, string>();

        /// <summary>
        /// Private constructor to enforce creation through factory methods
        /// </summary>
        private Entities() { }

        /// <summary>
        /// Creates a new entity with the specified parameters
        /// </summary>
        /// <param name="charId">The character's unique identifier</param>
        /// <param name="creatorNote">The creator's note about the character</param>
        /// <param name="charAttributes">The character's attributes</param>
        /// <returns>A new Entities instance</returns>
        public static Entities CreateEntityFromParameters(int charId, string creatorNote, Dictionary<string, string> charAttributes)
        {
            return new Entities()
            {
                CharId = charId,
                CreatorNote = creatorNote,
                CharAttributes = charAttributes
            };
        }

        /// <summary>
        /// Creates a new character with an auto-incremented ID
        /// </summary>
        /// <returns>A new Entities instance with a unique ID</returns>
        public static Entities CreateCharacter()
        {
            if (CharCount == 0 && File.Exists(PastIdPath))
            {
                string content = File.ReadAllText(PastIdPath);
                if (int.TryParse(content, out int oldValue))
                {
                    CharCount = oldValue;
                }
            }

            var newChar = new Entities
            {
                CharId = CharCount
            };
            CharCount++;
            
            File.WriteAllText(PastIdPath, CharCount.ToString());
            return newChar;
        }

        /// <summary>
        /// Gets a list of all attribute names defined for this character
        /// </summary>
        /// <returns>A list of attribute names</returns>
        public List<string> GetCharAttributesName()
        {
            return CharAttributes.Keys.ToList();
        }

        /// <summary>
        /// Sets the value of a specific attribute
        /// </summary>
        /// <param name="key">The name of the attribute to set</param>
        /// <param name="value">The value to assign to the attribute</param>
        public void SetAttribute(string key, string value)
        {
            CharAttributes[key] = value;
        }

        /// <summary>
        /// Gets the value of a specific attribute
        /// </summary>
        /// <param name="key">The name of the attribute to retrieve</param>
        /// <returns>The value of the specified attribute, or an empty string if not found</returns>
        public string GetAttribute(string key)
        {
            return CharAttributes.ContainsKey(key) ? CharAttributes[key] : "";
        }

        /// <summary>
        /// Updates the character's JSON representation in the specified folder
        /// </summary>
        /// <param name="pathToEntitiesFolder">The path to the folder where the JSON file should be saved</param>
        public void UpdateJson(string pathToEntitiesFolder)
        { 
            string filePath = GetCharacterFilePath(pathToEntitiesFolder);
            string json = JsonSerializer.Serialize(this, jsonSerializerOptions);
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Gets the file path for this character's JSON file
        /// </summary>
        /// <param name="pathToEntitiesFolder">The base path to the entities folder</param>
        /// <returns>The full path to the character's JSON file</returns>
        private string GetCharacterFilePath(string pathToEntitiesFolder)
        {
            return Path.Combine(pathToEntitiesFolder, $"character_{CharId}.json");
        }

        /// <summary>
        /// Loads a character from its JSON file
        /// </summary>
        /// <param name="id">The ID of the character to load</param>
        /// <param name="pathToEntitiesFolder">The path to the folder containing the character's JSON file</param>
        /// <returns>The loaded character, or null if not found</returns>
        public static Entities LoadCharacter(int id, string pathToEntitiesFolder)
        {
            string filePath = Path.Combine(pathToEntitiesFolder, $"character_{id}.json");
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Character {id} not found.");
                return null;
            }

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<Entities>(json, jsonSerializerOptions);
        }

        /// <summary>
        /// Prints the character's details to the console
        /// </summary>
        public void PrintCharacterStats()
        {
            Console.WriteLine($"Character : {CharId}");
            Console.WriteLine($"Creator's note : {CreatorNote}");
            Console.WriteLine("Attributes :");
            foreach (var attribute in CharAttributes)
            {
                Console.WriteLine($"\t{attribute.Key}: {attribute.Value}");
            }
            Console.WriteLine();
        }
    }
}

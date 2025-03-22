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
    public class Entities : Core.Entities.IEntities
    {
        // Path to the file where the count of characters is stored, to keep track of the last id used
        // So the path to a config file or folder
        // Could also be specific to each project (project1 having its own count file and characters folder, project2 having its own etc)
        public static string PastIdPath { get; private set; }

        public static int CharCount { get; private set; } = 0;
        private static JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions { Converters = { new JsonEntitiesConverter() }, PropertyNameCaseInsensitive = true, WriteIndented = true };
        // Unique identifier for each character
        public int CharId { get; private set; }
        // Note for the creator of the character ("Friend's self-insert character"), 'outside' of the world context
        public string CreatorNote { get; set; } = "";
        // Key: attribute name (e.g. "Name", "Age", "Job"), Value: attribute value (e.g. "John", "20s", "Warrior")
        public Dictionary<string, string> CharAttributes { get; private set; } = new Dictionary<string, string>();


        private Entities() { }
        public static Entities CreateEntityFromParameters(int charId, string creatorNote, Dictionary<string, string> charAttributes)
        {
            var entity = new Entities()
            {
                CharId = charId,
                CreatorNote = creatorNote,
                CharAttributes = charAttributes
            };
            return entity;
        }
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

            Entities newChar = new Entities
            {
                CharId = CharCount
            };
            CharCount++;
            
            File.WriteAllText(PastIdPath, CharCount.ToString());
            return newChar;
        }
        public List<string> GetCharAttributesName()
        {
            return CharAttributes.Keys.ToList();
        }
        public void SetAttribute(string key, string value)
        {
                CharAttributes[key] = value;
        }

        public string GetAttribute(string key)
        {
            return CharAttributes.ContainsKey(key) ? CharAttributes[key] : "";
        }

        public void UpdateJson(string pathToEntitiesFolder)
        { 
            string filePath = GetCharacterFilePath(pathToEntitiesFolder);
            string json = JsonSerializer.Serialize(this, jsonSerializerOptions);
            File.WriteAllText(filePath, json);
        }

        private string GetCharacterFilePath(string pathToEntitiesFolder)
        {
            return Path.Combine(pathToEntitiesFolder, $"character_{CharId}.json");
        }

        public static Entities LoadCharacter(int id,string pathToEntitiesFolder)
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

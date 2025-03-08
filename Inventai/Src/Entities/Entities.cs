using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Inventai.Src.Entities
{
    class Entities : Core.Entities.IEntities
    {
        public static int charCount { get; private set; } = 0;
        // Unique identifier for each character
        public int CharId { get; private set; }
        // Note for the creator of the character ("Friend's self-insert character"), 'outside' of the world context
        public string CreatorNote { get; set; } = "";
        private JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
        // Key: attribute name (e.g. "Name", "Age", "Job"), Value: attribute value (e.g. "John", "20s", "Warrior")
        public Dictionary<string, string> CharAttributes { get; private set; } = new Dictionary<string, string>();


        private Entities()
        {
            CharId = charCount;
            charCount++;
        }
        public static Entities CreateCharacter()
        {
            return new Entities();
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

        public static Entities? LoadCharacter(int id,string pathToEntitiesFolder)
        {
            string filePath = Path.Combine(pathToEntitiesFolder, $"character_{id}.json");

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Character {id} not found.");
                return null;
            }

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<Entities>(json);
        }

        public void PrintCharacterStats()
        {
            foreach (var attribute in CharAttributes)
            {
                Console.WriteLine($"{attribute.Key}: {attribute.Value}");
            }
        }
    }
}

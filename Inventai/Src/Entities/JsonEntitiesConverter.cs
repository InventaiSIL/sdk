using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Inventai.Src.Entities
{
    public class JsonEntitiesConverter : JsonConverter<Entities>
    {
        public override Entities Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Start reading the JSON
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                var rootElement = doc.RootElement;

                // Extract CharId and CreatorNote, handle missing keys gracefully
                int charId = rootElement.TryGetProperty("CharId", out var charIdElement) ? charIdElement.GetInt32() : 0;
                string creatorNote = rootElement.TryGetProperty("CreatorNote", out var creatorNoteElement) ? creatorNoteElement.GetString() : "";

                // Extract CharAttributes (dictionary), handle missing key gracefully
                var charAttributes = new Dictionary<string, string>();
                if (rootElement.TryGetProperty("CharAttributes", out var charAttributesElement))
                {
                    foreach (var attribute in charAttributesElement.EnumerateObject())
                    {
                        charAttributes[attribute.Name] = attribute.Value.GetString();
                    }
                }

                // Create the entity using the factory method
                return Entities.CreateEntityFromParameters(charId, creatorNote, charAttributes);
            }
        }


        public override void Write(Utf8JsonWriter writer, Entities value, JsonSerializerOptions options)
        {
            // Instead of calling JsonSerializer.Serialize(), just manually serialize the properties.
            writer.WriteStartObject();

            writer.WriteNumber("CharId", value.CharId);        // Serialize CharId
            writer.WriteString("CreatorNote", value.CreatorNote);  // Serialize CreatorNote

            writer.WriteStartObject("CharAttributes");  // Serialize CharAttributes (Dictionary)
            foreach (var attribute in value.CharAttributes)
            {
                writer.WriteString(attribute.Key, attribute.Value);
            }
            writer.WriteEndObject();

            writer.WriteEndObject();
        }
    }
}

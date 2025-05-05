using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Inventai.Src.Entities
{
    /// <summary>
    /// Custom JSON converter for the Entities class
    /// </summary>
    public class JsonEntitiesConverter : JsonConverter<Entities>
    {
        /// <summary>
        /// Reads and converts JSON to an Entities object
        /// </summary>
        /// <param name="reader">The JSON reader</param>
        /// <param name="typeToConvert">The type to convert to</param>
        /// <param name="options">The JSON serializer options</param>
        /// <returns>A new Entities instance created from the JSON data</returns>
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

        /// <summary>
        /// Writes an Entities object to JSON
        /// </summary>
        /// <param name="writer">The JSON writer</param>
        /// <param name="value">The Entities object to write</param>
        /// <param name="options">The JSON serializer options</param>
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

using Inventai.Core.Character;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace InventaiNovel
{
    public class Scene
    {
        public int Id { get; set; }
        public List<Character> Characters { get; set; } = new List<Character>();

        [JsonPropertyName("backgroundImage")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required byte[] BackgroundImage { get; set; }
        public required List<string> MetaData { get; set; }
        public required string Narrative { get; set; }
        public required List<string> Options { get; set; }

        public string? ToJson()
        {
            try
            {
                return JsonSerializer.Serialize(new
                {
                    Id,
                    Characters,
                    BackgroundImage = Encoding.UTF8.GetString(BackgroundImage),
                    MetaData,
                    Narrative,
                    Options
                });
            }
            catch (Exception e)
            {
                Console.WriteLine("Error serializing scene to JSON: " + e.Message);
                return null;
            }
        }
    }
}

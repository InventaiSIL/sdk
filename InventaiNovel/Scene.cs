using Inventai.Core.Character;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace InventaiNovel
{
    public class Scene
    {
        /// <summary>
        /// Unique identifier for the scene
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Characters present in this scene
        /// </summary>
        public List<Character> Characters { get; set; } = new List<Character>();

        /// <summary>
        /// The narrative text for this scene
        /// </summary>
        public required string Narrative { get; set; }

        /// <summary>
        /// Available choices in this scene
        /// </summary>
        public required List<string> Options { get; set; }

        /// <summary>
        /// Background image for the scene
        /// </summary>
        [JsonPropertyName("backgroundImage")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required byte[] BackgroundImage { get; set; }

        /// <summary>
        /// Metadata for the scene
        /// </summary>
        public required List<string> MetaData { get; set; }

        /// <summary>
        /// Previous choices that led to this scene
        /// Key: Scene ID, Value: Choice Index
        /// </summary>
        public Dictionary<int, int> PreviousChoices { get; set; } = new Dictionary<int, int>();

        /// <summary>
        /// Scene-specific context based on previous choices
        /// </summary>
        public string SceneContext { get; set; } = string.Empty;

        /// <summary>
        /// Next scene IDs for each choice
        /// </summary>
        public List<int> NextSceneIds { get; set; } = new List<int>();

        public Scene()
        {
            Options = new List<string>();
            MetaData = new List<string>();
        }

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

using Inventai.ImageAgents;
using Inventai.TextAgents;
using InventaiNovel;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            Console.WriteLine("Starting the program...");
            await TestGenerateNovelJson();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    static async Task TestGenerateNovelJson()
    {
        try
        {
            string? openaiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            if (string.IsNullOrEmpty(openaiApiKey))
            {
                Console.WriteLine("OPENAI - OPENAI_API_KEY environment variable is not set");
                return;
            }

            string? segmindApiKey = Environment.GetEnvironmentVariable("SEGMIND_API_KEY");
            if (string.IsNullOrEmpty(segmindApiKey))
            {
                Console.WriteLine("LOCAL IMAGE - SEGMIND_API_KEY environment variable is not set");
                return;
            }

            Console.WriteLine("segmindApiKey: " + segmindApiKey);

            Console.WriteLine("Initializing agents...");
            TextAgentOpenAI textAgent = new("gpt-3.5-turbo", openaiApiKey);
            ImageAgentStableDiffusion imageAgent = new("https://api.segmind.com/v1/stable-diffusion-3.5-turbo-txt2img", segmindApiKey);

            NovelCreationRequest novelCreationRequest = new() { 
                CharacterCreationRequest = new()
                { Context = "Medieval ages", Prompt = "History of a mage hunting a dragon", NumCharacters = 1 },
                NumScenes = 1,
                Prompt = "History of a mage hunting a dragon",
                Context = "Medieval ages"
            };

            Console.WriteLine("Creating novel manager...");
            InventaiNovelManager novelManager = new(textAgent, imageAgent);

            Console.WriteLine("Creating novel...");
            novelManager.CreateNovel(novelCreationRequest);

            var basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "nov");
            Console.WriteLine($"Saving novel to: {basePath}");

            await novelManager.SaveNovel(basePath);
            
            Console.WriteLine("Exporting to Ren'Py format...");
            await novelManager.ExportToRenpy(basePath);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in TestGenerateNovelJson: {e.Message}");
            Console.WriteLine($"Stack trace: {e.StackTrace}");
            throw;
        }
    }
}
using Inventai.ImageAgents;
using Inventai.TextAgents;
using InventaiNovel;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System.Text.Json;

static async Task TextGenerateNovelJson()
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

    TextAgentOpenAI textAgent = new("gpt-3.5-turbo", openaiApiKey);
    ImageAgentStableDiffusion imageAgent = new("https://api.segmind.com/v1/stable-diffusion-3.5-turbo-txt2img", segmindApiKey);

    NovelCreationRequest novelCreationRequest = new() { 
        CharacterCreationRequest = new()
        { Context = "Medieval ages", Prompt = "History of a mage hunting a dragon", NumCharacters = 1 },
        NumScenes = 1,
        Prompt = "History of a mage hunting a dragon",
        Context = "Medieval ages"
    };

    InventaiNovelManager novelManager = new(textAgent, imageAgent);

    novelManager.CreateNovel(novelCreationRequest);

    try
    {
        var basePath = "C:\\Users\\atepi\\nov\\";
        await novelManager.SaveNovel(basePath);

        // save the novel to a file
        try
        {
            try { Directory.CreateDirectory(basePath); } catch { }

            Console.WriteLine("Saving the novel to a file");
            var novelPath = basePath + "novel.json";
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.Preserve // Handle circular references
            };
            await File.WriteAllTextAsync(novelPath, System.Text.Json.JsonSerializer.Serialize(novelManager.GetScenes(), options));

            Console.WriteLine("Novel saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving the novel: " + ex.Message);
            throw;
        }
    }
    catch (Exception e)
    {
        Console.WriteLine("Error saving the novel: " + e.Message);
    }
}

static async Task Main(string[] args)
{
    TextGenerateNovelJson();
}

Main(args);
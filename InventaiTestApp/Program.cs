<<<<<<< Updated upstream
﻿using Inventai.TextAgents;
using Inventai.Core;

try
{
    TextAgentOpenAI agent = new(EOpenAITextModels.GPT4omini);
    Console.WriteLine(agent.CompleteMessage("Say 'This is a test.'"));
=======
﻿using Inventai.ImageAgents;
using Inventai.TextAgents;

try
{
    string? openaiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
    if (string.IsNullOrEmpty(openaiApiKey))
        Console.WriteLine("OPENAI - OPENAI_API_KEY environment variable is not set");
    else
    {
        TextAgentOpenAI agent = new("gpt-3.5-turbo", openaiApiKey);
        Console.WriteLine(agent.CompleteMessage("Say 'OPENAI - This is a test.'"));
    }

    try
    {
        TextAgentLocalLlm agent2 = new("llama3.2", new Uri("http://localhost:11434/v1/"));
        Console.WriteLine(agent2.CompleteMessage("Say 'LOCAL LLM - This is a test.'"));
    }
    catch
    {
        Console.WriteLine("LOCAL LLM - Error running local LLM");
    }

    string? segmindApiKey = Environment.GetEnvironmentVariable("SEGMIND_API_KEY");
    if (string.IsNullOrEmpty(segmindApiKey))
        Console.WriteLine("LOCAL IMAGE - SEGMIND_API_KEY environment variable is not set");
    else
    {
        ImageAgentStableDiffusion segmindSdAgent = new("https://api.segmind.com/v1/stable-diffusion-3.5-turbo-txt2img", segmindApiKey);
        byte[] segmindImage = await segmindSdAgent.GenerateImageAsync(new()
        {
            Prompt = "A group of children playing football",
        });

        await File.WriteAllBytesAsync("segmindImage.jpg", segmindImage);
    }
>>>>>>> Stashed changes
}
catch (Exception e)
{
    Console.WriteLine(e);
}

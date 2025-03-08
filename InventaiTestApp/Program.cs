using Inventai.TextAgents;
﻿using Inventai.ImageAgents;

static void TestOpenai()
{
    string? openaiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
    if (string.IsNullOrEmpty(openaiApiKey))
        Console.WriteLine("OPENAI - OPENAI_API_KEY environment variable is not set");
    else
    {
        TextAgentOpenAI agent = new("gpt-3.5-turbo", openaiApiKey);
        Console.WriteLine(agent.CompleteMessage("Say 'OPENAI - This is a test.'"));
    }
}

static void TestLocalLlm()
{
    try
    {
        TextAgentLocalLlm agent = new("llama3.2", new Uri("http://localhost:11434/v1/"));
        Console.WriteLine(agent.CompleteMessage("Say 'LOCAL LLM - This is a test.'"));
    }
    catch
    {
        Console.WriteLine("LOCAL LLM - Error running local LLM");
    }
}

static async Task TestSegmind()
{
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
        Console.WriteLine(segmindImage.Length);
    }
}

static void TestDiscussionContextChoices()
{
    string? openaiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
    if (string.IsNullOrEmpty(openaiApiKey))
        Console.WriteLine("OPENAI - OPENAI_API_KEY environment variable is not set");
    else
    {
        TextAgentOpenAI agent = new("gpt-3.5-turbo", openaiApiKey);

        Inventai.Src.Discussion.DiscussionContextManager discussionContextManager = new(agent);

        Inventai.Core.Discussion.ContextualChoicesRequest request = new()
        {
            Prompt = "Choices to be good or bad person",
            Context = "You are a person who is trying to be good",
            NumChoices = 3
        };

        Inventai.Core.Discussion.ContextualChoicesResponse response = discussionContextManager.GenerateContextualChoices(request);

        Console.WriteLine("Choices: " + response.Choices.ToString());
    }
}

static void TextDiscussionContextChat()
{
    string? openaiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
    if (string.IsNullOrEmpty(openaiApiKey))
        Console.WriteLine("OPENAI - OPENAI_API_KEY environment variable is not set");
    else
    {
        TextAgentOpenAI agent = new("gpt-3.5-turbo", openaiApiKey);

        Inventai.Src.Discussion.DiscussionContextManager discussionContextManager = new(agent);

        Inventai.Core.Discussion.EntitiesChatRequest request = new()
        {
            Prompt = "Entities chat for a person",
            Context = "You are a person who is trying to be good",
            Entities = [
                new Inventai.Core.Discussion.EntityExample()
                {
                    Id = "1",
                    Name = "Person 1"
                },
                new Inventai.Core.Discussion.EntityExample()
                {
                    Id = "2",
                    Name = "Person 2"
                }
            ]
        };

        Inventai.Core.Discussion.EntitiesChatResponse response = discussionContextManager.GenerateEntitiesChat(request);

        Console.WriteLine("Entities: " + response.ToString());
    }
}

static async Task Main(string[] args)
{
    //TestOpenai();
    //TestLocalLlm();
    //await TestSegmind();

    TestDiscussionContextChoices();
    TextDiscussionContextChat();
}

Main(args);
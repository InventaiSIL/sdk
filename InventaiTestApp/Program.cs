using Inventai.TextAgents;

try
{
    string? apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
    if (string.IsNullOrEmpty(apiKey))
        throw new InvalidOperationException("OPENAI_API_KEY environment variable is not set");
    TextAgentOpenAI agent = new TextAgentOpenAI("gpt-3.5-turbo", apiKey);
    Console.WriteLine(agent.CompleteMessage("Say 'This is a test.'"));

    TextAgentLocalLLM agent2 = new TextAgentLocalLLM("llama3.2", new Uri("http://localhost:11434/v1/"));
    Console.WriteLine(agent2.CompleteMessage("Say 'This is a test.'"));
}
catch (Exception e)
{
    Console.WriteLine(e);
}

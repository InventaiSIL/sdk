using Inventai.TextAgents;
using Inventai.Core;

try
{
    TextAgentOpenAI agent = new(EOpenAITextModels.GPT4omini);
    Console.WriteLine(agent.CompleteMessage("Say 'This is a test.'"));
}
catch (Exception e)
{
    Console.WriteLine(e);
}

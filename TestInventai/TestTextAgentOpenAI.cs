using Inventai.Core;
using Inventai.TextAgents;

namespace TestInventai
{
    public class Tests
    {
        TextAgentOpenAI Agent { get; set; }

        [SetUp]
        public void Setup()
        {
            string? apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
                throw new InvalidOperationException("OPENAI_API_KEY environment variable is not set");

            Agent = new(EOpenAITextModels.GPT4omini, apiKey);
        }

        [Test]
        public void TestSay()
        {
            Assert.That(Agent.CompleteMessage("Say 'This is a test.'"), Is.EqualTo("This is a test."));
        }
    }
}
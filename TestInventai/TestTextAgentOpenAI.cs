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
            Agent = new(EOpenAITextModels.GPT4omini);
        }

        [Test]
        public void TestSay()
        {
            Assert.That(Agent.CompleteMessage("Say 'This is a test.'"), Is.EqualTo("This is a test."));
        }
    }
}
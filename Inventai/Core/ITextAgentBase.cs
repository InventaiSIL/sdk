namespace Inventai.Core;

/// <summary>
/// Base interface to be implemented by the text agents
/// </summary>
public interface ITextAgent
{
    /// <summary>
    /// Completes <paramref name="pMessage"/> as per the LLM model
    /// </summary>
    /// <param name="pMessage"></param>
    /// <returns></returns>
    public string CompleteMessage(string pMessage);
}

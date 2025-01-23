namespace Inventai.Core;

/// <summary>
/// Exception raised when a chat client encounters issues
/// </summary>
/// <param name="pMessage"></param>
public class ChatClientException(string? pMessage) : Exception(pMessage)
{
}

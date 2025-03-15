using System;

namespace Inventai.Core
{
    /// <summary>
    /// Exception raised when a chat client encounters issues
    /// </summary>
    public class ChatClientException : Exception
    {
        public ChatClientException(string pMessage) : base(pMessage)
        {
        }
    }
}

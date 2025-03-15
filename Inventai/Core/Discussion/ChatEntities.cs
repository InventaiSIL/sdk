using Inventai.Core.Character;
using System.Collections.Generic;
using System.Text.Json;

namespace Inventai.Core.Discussion
{
    /// <summary>
    /// Entity messsage
    /// </summary>
    public class EntityMessage
    {
        /// <summary>
        /// The entity sending the message
        /// </summary>
        public string EntityId { get; set; }

        /// <summary>
        /// The message
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// Entities discussion
    /// </summary>
    public class EntitiesChatResponse
    {
        /// <summary>
        /// List of participants
        /// </summary>
        public List<string> Participants { get; set; }

        /// <summary>
        /// List of messages
        /// </summary>
        public List<EntityMessage> EntitiesMessages { get; set; }

        
        public virtual string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }

    /// <summary>
    /// Request object for generating entities Chat
    /// </summary>
    public class EntitiesChatRequest
    {
        /// <summary>
        /// Prompt for which entities are to be generated
        /// </summary>
        public string Prompt { get; set; }

        /// <summary>
        /// Context for the prompt
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// Entities to generate the chats for
        /// </summary>
        public Character.Character[] Entities { get; set; }

        /// <summary>
        /// Number of messages to generate
        /// </summary>
        public int NumMessages { get; set; }
    }
}

namespace Inventai.Core.Discussion
{
    /// <summary>
    /// Base interface to be implemented by the text agents
    /// </summary>
    public interface IChatInteractableEntityBase
    {
        /// <summary>
        /// Unique identifier of the entity
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Name of the entity
        /// </summary>
        public string Name { get; }
    }

    public record class EntityExample : IChatInteractableEntityBase
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// Entities messsages
    /// </summary>
    public record class EntityMessage
    {
        /// <summary>
        /// The entity sending the message
        /// </summary>
        public IChatInteractableEntityBase Entity { get; set; }

        /// <summary>
        /// The message
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// Entities discussion
    /// </summary>
    public record class EntitiesChatResponse
    {
        /// <summary>
        /// List of participants
        /// </summary>
        public List<string> Participants { get; set; }

        /// <summary>
        /// List of messages
        /// </summary>
        public List<EntityMessage> EntitiesMessage { get; set; }
    }

    /// <summary>
    /// Request object for generating entities Chat
    /// </summary>
    public record class EntitiesChatRequest
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
        /// Entities to generate
        /// </summary>
        public IChatInteractableEntityBase[] Entities { get; set; }

        /// <summary>
        /// Number of messages to generate
        /// </summary>
        public int NumMessages { get; set; }
    }
}

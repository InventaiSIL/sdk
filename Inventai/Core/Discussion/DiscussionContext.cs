namespace Inventai.Core.Discussion
{
    /// <summary>
    /// Discussion context interface
    /// </summary>
    internal interface IDiscussionContext
    {
        /// <summary>
        /// Generates a list of choices from a prompt <paramref name="pRequest"/>
        /// </summary>
        /// <param name="pRequest"></param>
        /// <returns></returns>
        ContextualChoicesResponse GenerateContextualChoices(ContextualChoicesRequest pRequest);

        /// <summary>
        /// Generates a chats message from a prompt <paramref name="pRequest"/>
        /// </summary>
        /// <param name="pRequest"></param>
        /// <returns></returns>
        EntitiesChatResponse GenerateEntitiesChat(EntitiesChatRequest pRequest);
    }
}


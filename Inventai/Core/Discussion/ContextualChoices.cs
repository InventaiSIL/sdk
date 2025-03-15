namespace Inventai.Core.Discussion
{
    /// <summary>
    /// Request object for generating contextually relevant choices
    /// </summary>
    public class ContextualChoicesRequest
    {
        /// <summary>
        /// Prompt for which choices are to be generated
        /// </summary>
        public string Prompt { get; set; }

        /// <summary>
        /// Context for the prompt
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// Number of choices to generate
        /// </summary>
        public int NumChoices { get; set; }
    }

    /// <summary>
    /// Response object for generating contextually relevant choices
    /// </summary>
    public class ContextualChoicesResponse
    {
        /// <summary>
        /// List of generated choices
        /// </summary>
        public string[] Choices { get; set; }
    }
}
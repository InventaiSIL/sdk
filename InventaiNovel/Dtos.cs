using Inventai.Core.Character;

namespace InventaiNovel
{
    /// <summary>
    /// Represents a request to create a novel.
    /// </summary>
    public class NovelCreationRequest
    {
        /// <summary>
        /// Gets or sets the character creation request.
        /// </summary>
        public required CharacterCreationRequest CharacterCreationRequest { get; set; }

        /// <summary>
        /// Gets or sets the number of scenes in the novel.
        /// </summary>
        public int NumScenes { get; set; }

        /// <summary>
        /// Gets or sets the prompt for the novel.
        /// </summary>
        public required string Prompt { get; set; }

        /// <summary>
        /// Gets or sets the context for the novel.
        /// </summary>
        public required string Context { get; set; }
    }
}

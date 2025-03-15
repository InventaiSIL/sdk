using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventai.Core.Character
{
    /// <summary>
    /// Base interface to be implemented by the text agents
    /// </summary>
    public interface ICharacter
    {
        /// <summary>
        /// Unique identifier of the entity
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Name of the entity
        /// </summary>
        string Name { get; }
    }

    public class Character : ICharacter
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }
        public string MetaData { get; set; }
    }

    public class CharacterCreationRequest
    {
        /// <summary>
        /// Description of the characters to be generated
        /// </summary>
        public string Prompt { get; set; }

        /// <summary>
        /// Context for the prompt
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// Number of characters to generate
        /// </summary>
        public int NumCharacters { get; set; }
    }
}

using Inventai.Core.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventaiNovel
{
    /// <summary>
    /// Represents a complete novel with its characters and scenes
    /// </summary>
    public class Novel
    {
        /// <summary>
        /// Gets or sets the list of characters in the novel
        /// </summary>
        public required List<Character> Characters { get; set; }

        /// <summary>
        /// Gets or sets the list of scenes that make up the novel's story
        /// </summary>
        public required List<Scene> Scenes { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Inventai.Core.Entities
{
    /// <summary>
    /// Interface defining the core functionality for entity management
    /// </summary>
    public interface IEntities
    {
        /// <summary>
        /// Gets the unique identifier of the character
        /// </summary>
        int CharId { get; }

        /// <summary>
        /// Gets or sets the creator's note about the character
        /// </summary>
        string CreatorNote { get; set; }

        /// <summary>
        /// Gets the dictionary of character attributes
        /// </summary>
        Dictionary<string, string> CharAttributes { get; }

        /// <summary>
        /// Retrieves a list of all attribute names defined for the character
        /// </summary>
        /// <returns>A list of attribute names</returns>
        List<string> GetCharAttributesName();

        /// <summary>
        /// Sets the value of a specific attribute
        /// </summary>
        /// <param name="key">The name of the attribute to set</param>
        /// <param name="value">The value to assign to the attribute</param>
        void SetAttribute(string key, string value);

        /// <summary>
        /// Gets the value of a specific attribute
        /// </summary>
        /// <param name="key">The name of the attribute to retrieve</param>
        /// <returns>The value of the specified attribute</returns>
        string GetAttribute(string key);

        /// <summary>
        /// Updates the entity's JSON representation in the specified folder
        /// </summary>
        /// <param name="pathToJsonFolder">The path to the folder where the JSON file should be saved</param>
        void UpdateJson(string pathToJsonFolder);

        //    static abstract Entities LoadCharacter(int id,string pathToEntityFolder);
    }
}

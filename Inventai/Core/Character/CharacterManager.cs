using Inventai.Core.Discussion;
using System.Collections.Generic;

namespace Inventai.Core.Character
{
    internal interface ICharacterManager
    {
        /// <summary>
        /// Generates a list of characters from a prompt <paramref name="pRequest"/>
        /// </summary>
        /// <param name="pRequest"></param>
        /// <returns></returns>
        List<Character> GenerateCharacters(CharacterCreationRequest pRequest);
    }
}

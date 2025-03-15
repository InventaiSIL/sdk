using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Inventai.Core.Entities
{
    interface IEntities
    {

            int CharId { get; }
        // "Friend's self-insert character"
        string CreatorNote { get; set; }
            Dictionary<string, string> CharAttributes { get; }
        // To retrieve the list of all type of attributes (so could return : "Job","Age",etc)    
        List<string> GetCharAttributesName();
            void SetAttribute(string key, string value);
            string GetAttribute(string key);
            void UpdateJson(string pathToJsonFolder);

        //    static abstract Entities LoadCharacter(int id,string pathToEntityFolder);
    }
}

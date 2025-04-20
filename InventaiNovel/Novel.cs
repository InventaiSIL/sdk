using Inventai.Core.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventaiNovel
{
    public class Novel
    {
        public required List<Character> Characters { get; set; }
        public required List<Scene> Scenes { get; set; }
    }
}

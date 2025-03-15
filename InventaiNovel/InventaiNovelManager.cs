using Inventai.Character;
using Inventai.Core;
using Inventai.Core.Character;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InventaiNovel
{
    /// <summary>
    /// Novel manager
    /// </summary>
    public class InventaiNovelManager
    {
        /// <summary>
        /// Character manager
        /// </summary>
        private readonly CharacterManager m_CharacterManager;

        /// <summary>
        /// Characters of the novel
        /// </summary>
        private List<Inventai.Core.Character.Character> m_Characters;

        /// <summary>
        /// Scenes of the novel
        /// </summary>
        private List<Scene> m_Scenes;

        /// <summary>
        /// The general context of the novel
        /// </summary>
        private string m_GeneralContext;

        /// <summary>
        /// The text agent to use
        /// </summary>
        private readonly ITextAgent m_TextAgent;

        /// <summary>
        /// The image agent to use
        /// </summary>
        private readonly IImageAgent m_ImageAgent;

        /// <summary>
        /// Ctor
        /// </summary>
        public InventaiNovelManager(ITextAgent pTextAgent, IImageAgent pImageAgent)
        {
            m_TextAgent = pTextAgent;
            m_ImageAgent = pImageAgent;
            m_CharacterManager = new CharacterManager(pTextAgent, pImageAgent);
            m_Characters = new List<Inventai.Core.Character.Character>();
            m_Scenes = new List<Scene>();
            m_GeneralContext = "";
            Console.WriteLine("Inventai Novel Manager created");
        }

        /// <summary>
        /// Create a novel
        /// </summary>
        /// <param name="pRequest"></param>
        public void CreateNovel(NovelCreationRequest pRequest)
        {
            // Generate the characters
            m_Characters = m_CharacterManager.GenerateCharacters(pRequest.CharacterCreationRequest);
            m_GeneralContext = pRequest.Prompt + " with context: " + pRequest.Context;
            // Generate the scenes
            Console.WriteLine("Creating " + pRequest.NumScenes + " scenes for the novel with the characters: " + string.Join(", ", m_Characters.Select(c => c.Name)));
            for (int i = 0; i < pRequest.NumScenes; i++)
            {
                CreateNextScene(m_Characters);
            }
        }

        /// <summary>
        /// Create the next scene
        /// </summary>
        /// <param name="pCharacters"></param>
        public void CreateNextScene(List<Character> pCharacters)
        {
            Console.WriteLine("Creating the next scene for the characters: " + string.Join(", ", pCharacters.Select(c => c.Name)));
            var optionsString = m_TextAgent.CompleteMessage("Create the options to choose for the player for the scene with the characters: "
                    + string.Join(", ", m_Characters.Select(c => c.Name))
                    + " with the general context: " + m_GeneralContext
                    + " and having the metadata: " + string.Join(", ", m_Characters.Select(c => c.MetaData), m_GeneralContext));
            List<string>? options = optionsString.Split("\n").ToList();

            // Generate the scene
            var scene = new Scene
            {
                Id = m_Scenes.Count + 1,
                Characters = pCharacters,
                BackgroundImage = m_ImageAgent.GenerateCharacterImageAsync(
                    "Create an image for the scene with the characters: "
                    + string.Join(", ", m_Characters.Select(c => c.Name))
                    + " with the general context: " + m_GeneralContext
                    + " and having the metadata: " + string.Join(", ", m_Characters.Select(c => c.MetaData)), m_GeneralContext).Result,
                MetaData = m_Characters.Select(c => c.MetaData).ToList(),
                Narrative = m_TextAgent.CompleteMessage("Create the narrative for the scene with the characters: "
                    + string.Join(", ", m_Characters.Select(c => c.Name))
                    + " with the general context: " + m_GeneralContext
                    + " and having the metadata: " + string.Join(", ", m_Characters.Select(c => c.MetaData), m_GeneralContext)),
                Options = options!
            };
            m_Scenes.Add(scene);
            Console.WriteLine("Scene created successfully");
        }

        public List<Scene> GetScenes()
        {
            return m_Scenes;
        }

        /// <summary>
        /// Saves the novel to a file and returns the path, creates the images for the characters and scenes
        /// </summary>
        /// <returns></returns>
        public async Task SaveNovel(string pBasePath)
        {
            Console.WriteLine("Saving the novel to a file");

            try { Directory.CreateDirectory(pBasePath + "characters"); } catch { }
            foreach (var character in m_Characters)
            {
                // save character image to file
                var characterImage = character.Image;
                var characterImagePath = pBasePath + "characters/" + character.Name + ".jpg";
                await File.WriteAllBytesAsync(characterImagePath, characterImage);
            }
            Console.WriteLine("Characters saved successfully.");

            try { Directory.CreateDirectory(pBasePath + "scenes"); } catch { }
            foreach (var scene in m_Scenes)
            {
                // save scene image to file
                var sceneImage = scene.BackgroundImage;
                var sceneImagePath = pBasePath + "scenes/scene" + scene.Id + ".jpg";
                await File.WriteAllBytesAsync(sceneImagePath, sceneImage);
            }
            Console.WriteLine("Scenes saved successfully.");

            //// replace the image byte arrays with the image paths
            //foreach (var character in m_Characters)
            //{
            //    character.Image = Encoding.UTF8.GetBytes("characters/" + character.Name + ".jpg");
            //}
            //foreach (var scene in m_Scenes)
            //{
            //    scene.BackgroundImage = Encoding.UTF8.GetBytes("scenes/scene" + scene.Id + ".jpg");
            //}
        }
    }
}

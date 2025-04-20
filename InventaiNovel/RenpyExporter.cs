using Inventai.Core.Character;
using System.Text;

namespace InventaiNovel
{
    /// <summary>
    /// Handles exporting novel content to Ren'Py format
    /// </summary>
    public class RenpyExporter
    {
        private readonly List<Scene> m_Scenes;
        private readonly List<Character> m_Characters;
        private readonly string m_GeneralContext;
        private readonly StreamWriter m_Writer;

        public RenpyExporter(List<Scene> scenes, List<Character> characters, string generalContext, StreamWriter writer)
        {
            m_Scenes = scenes;
            m_Characters = characters;
            m_GeneralContext = generalContext;
            m_Writer = writer;
        }

        /// <summary>
        /// Export the novel to Ren'Py format
        /// </summary>
        public async Task ExportToRenpy()
        {
            try
            {
                await WriteCharacterDefinitions();
                await WriteImageDefinitions();
                await WriteStory();
                await WriteEndings();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RenpyExporter: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private async Task WriteCharacterDefinitions()
        {
            await m_Writer.WriteLineAsync("# Character definitions");
            await m_Writer.WriteLineAsync("init python:");
            await m_Writer.WriteLineAsync("    narrator = Character(None, kind=nvl)");
            foreach (var character in m_Characters)
            {
                var safeName = character.Name.ToLower().Replace(" ", "_");
                await m_Writer.WriteLineAsync($"    {safeName} = Character(\"{character.Name}\", kind=adv)");
            }
            await m_Writer.WriteLineAsync();
        }

        private async Task WriteImageDefinitions()
        {
            await m_Writer.WriteLineAsync("# Image definitions");
            foreach (var character in m_Characters)
            {
                var safeName = character.Name.ToLower().Replace(" ", "_");
                await m_Writer.WriteLineAsync($"image {safeName} = \"{Path.Combine("images", "characters", $"{safeName}.jpg").Replace("\\", "/")}\"");
            }
            foreach (var scene in m_Scenes)
            {
                await m_Writer.WriteLineAsync($"image scene{scene.Id} = \"{Path.Combine("images", "scenes", $"scene{scene.Id}.jpg").Replace("\\", "/")}\"");
            }
            await m_Writer.WriteLineAsync();
        }

        private async Task WriteStory()
        {
            await m_Writer.WriteLineAsync("# The story starts here");
            await m_Writer.WriteLineAsync("init python:");
            await m_Writer.WriteLineAsync("    previous_choices = {}");
            await m_Writer.WriteLineAsync();
            await m_Writer.WriteLineAsync("label start:");
            await m_Writer.WriteLineAsync("    $ previous_choices = {}");
            await m_Writer.WriteLineAsync();

            foreach (var scene in m_Scenes)
            {
                await WriteScene(m_Scenes.IndexOf(scene));
            }
        }

        private async Task WriteScene(int sceneIndex)
        {
            var scene = m_Scenes[sceneIndex];
            
            // Generate a unique label based on the scene ID and previous choices
            string choicePath = string.Join("_", scene.PreviousChoices
                .OrderBy(kv => kv.Key)
                .Select(kv => $"c{kv.Key}_{kv.Value}"));
            
            string sceneLabel = string.IsNullOrEmpty(choicePath) 
                ? $"scene_{scene.Id}" 
                : $"scene_{scene.Id}_{choicePath}";

            await m_Writer.WriteLineAsync($"    # Scene {scene.Id}");
            await m_Writer.WriteLineAsync($"label {sceneLabel}:");

            // Add conditional text based on previous choices if any exist
            if (scene.PreviousChoices.Any())
            {
                await m_Writer.WriteLineAsync("    # Check previous choices context");
                foreach (var choice in scene.PreviousChoices)
                {
                    await m_Writer.WriteLineAsync($"    $ choice_{choice.Key} = previous_choices.get({choice.Key}, -1)");
                }
            }

            await m_Writer.WriteLineAsync($"    scene scene{scene.Id}");
            await m_Writer.WriteLineAsync("    with fade");
            await m_Writer.WriteLineAsync();

            await WriteNarrative(scene);
            await WriteChoices(sceneIndex, scene, sceneLabel);
        }

        private async Task WriteNarrative(Scene scene)
        {
            var paragraphs = scene.Narrative.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(para => para.Trim())
                .Where(para => !string.IsNullOrWhiteSpace(para));

            foreach (var para in paragraphs)
            {
                var escapedPara = para
                    .Replace("\"", "\\\"")  // Escape double quotes
                    .Replace("\n", " ")     // Replace newlines with spaces
                    .Replace("\r", "");     // Remove carriage returns

                await m_Writer.WriteLineAsync($"    narrator \"{escapedPara}\"");
                await m_Writer.WriteLineAsync("    nvl clear");
            }
            await m_Writer.WriteLineAsync();
        }

        private async Task WriteChoices(int sceneIndex, Scene scene, string currentSceneLabel)
        {
            if (scene.Options != null && scene.Options.Any())
            {
                await m_Writer.WriteLineAsync("    menu:");
                for (int i = 0; i < scene.Options.Count; i++)
                {
                    var option = scene.Options[i];
                    await m_Writer.WriteLineAsync($"        \"{option}\":");

                    // Find the next scene based on this choice
                    var nextScene = m_Scenes.FirstOrDefault(s => 
                        s.PreviousChoices.ContainsKey(scene.Id) && 
                        s.PreviousChoices[scene.Id] == i);

                    if (nextScene != null)
                    {
                        // Generate the next scene's label
                        var nextChoicePath = string.Join("_", nextScene.PreviousChoices
                            .OrderBy(kv => kv.Key)
                            .Select(kv => $"c{kv.Key}_{kv.Value}"));
                        
                        var nextSceneLabel = string.IsNullOrEmpty(nextChoicePath) 
                            ? $"scene_{nextScene.Id}" 
                            : $"scene_{nextScene.Id}_{nextChoicePath}";

                        await m_Writer.WriteLineAsync($"            $ previous_choices[{scene.Id}] = {i}");
                        await m_Writer.WriteLineAsync($"            jump {nextSceneLabel}");
                    }
                    else if (sceneIndex == m_Scenes.Count - 1)
                    {
                        await m_Writer.WriteLineAsync($"            jump end_{i + 1}");
                    }
                    else
                    {
                        // If no specific next scene is found, create a default next scene label
                        var defaultNextSceneLabel = $"scene_{scene.Id + 1}_default";
                        await m_Writer.WriteLineAsync($"            jump {defaultNextSceneLabel}");
                    }
                }
                await m_Writer.WriteLineAsync();
            }
            else if (sceneIndex < m_Scenes.Count - 1)
            {
                var defaultNextSceneLabel = $"scene_{scene.Id + 1}_default";
                await m_Writer.WriteLineAsync($"    jump {defaultNextSceneLabel}");
            }
            else
            {
                await m_Writer.WriteLineAsync("    jump end_1");
            }
            await m_Writer.WriteLineAsync();
        }

        private async Task WriteEndings()
        {
            int numEndings = m_Scenes.Count > 0 && m_Scenes.Last().Options?.Count > 0
                ? m_Scenes.Last().Options.Count
                : 1;

            for (int i = 0; i < numEndings; i++)
            {
                await m_Writer.WriteLineAsync($"label end_{i + 1}:");
                await m_Writer.WriteLineAsync($"    narrator \"You reached the end of your journey through choice {i + 1}.\"");
                await m_Writer.WriteLineAsync("    return");
                await m_Writer.WriteLineAsync();
            }
        }
    }
} 
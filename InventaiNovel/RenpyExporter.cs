using Inventai.Core.Character;
using System.Text;
using System.Text.RegularExpressions;

namespace InventaiNovel
{
    /// <summary>
    /// Handles exporting novel content to Ren'Py format with improved narrative display
    /// </summary>
    public class RenpyExporter
    {
        private readonly List<Scene> m_Scenes;
        private readonly List<Character> m_Characters;
        private readonly string m_GeneralContext;
        private readonly StreamWriter m_Writer;
        private readonly int m_MaxDepth;

        /// <summary>
        /// Initializes a new instance of the RenpyExporter class
        /// </summary>
        /// <param name="scenes">List of scenes to export</param>
        /// <param name="characters">List of characters in the novel</param>
        /// <param name="generalContext">General context of the novel</param>
        /// <param name="writer">StreamWriter to write the Ren'Py script to</param>
        public RenpyExporter(List<Scene> scenes, List<Character> characters, string generalContext, int maxDepth, StreamWriter writer)
        {
            m_Scenes = scenes ?? throw new ArgumentNullException(nameof(scenes));
            m_Characters = characters ?? throw new ArgumentNullException(nameof(characters));
            m_GeneralContext = generalContext;
            m_Writer = writer ?? throw new ArgumentNullException(nameof(writer));
            m_MaxDepth = maxDepth;
        }

        /// <summary>
        /// Export the novel to Ren'Py format with improved styling
        /// </summary>
        public async Task ExportToRenpy()
        {
            try
            {
                await WriteInitialConfiguration();
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

        /// <summary>
        /// Writes initial configuration for the Ren'Py game
        /// </summary>
        private async Task WriteInitialConfiguration()
        {
            await m_Writer.WriteLineAsync("# Game configuration");
            await m_Writer.WriteLineAsync("define config.adv_nvl_transition = dissolve");
            await m_Writer.WriteLineAsync("define config.nvl_adv_transition = dissolve");
            await m_Writer.WriteLineAsync("define config.window_title = \"Visual Novel\"");
            await m_Writer.WriteLineAsync();

            // Style configuration for narrative text box
            await m_Writer.WriteLineAsync("init python:");
            await m_Writer.WriteLineAsync("    style.nvl_window = Style(style.default)");
            await m_Writer.WriteLineAsync("    style.nvl_window.background = Solid(\"#000000B0\")  # Semi-transparent black");
            await m_Writer.WriteLineAsync("    style.nvl_window.xpadding = 50");
            await m_Writer.WriteLineAsync("    style.nvl_window.ypadding = 30");
            await m_Writer.WriteLineAsync("    style.nvl_window.xmargin = 50");
            await m_Writer.WriteLineAsync("    style.nvl_window.bottom_margin = 50");  // Changed from ymargin to bottom_margin
            await m_Writer.WriteLineAsync("    style.nvl_window.top_padding = 0");  // Added to remove top padding
            await m_Writer.WriteLineAsync("    style.nvl_window.ypos = 0.8");  // Position the window lower on screen
            await m_Writer.WriteLineAsync("    style.nvl_window.yfill = False");  // Don't fill the whole height
            await m_Writer.WriteLineAsync("    style.nvl_window.yanchor = 1.0");  // Anchor to bottom
            await m_Writer.WriteLineAsync();
        }

        /// <summary>
        /// Writes character definitions to the Ren'Py script
        /// </summary>
        private async Task WriteCharacterDefinitions()
        {
            await m_Writer.WriteLineAsync("# Character definitions");
            await m_Writer.WriteLineAsync("init python:");
            await m_Writer.WriteLineAsync("    narrator = Character(None, kind=nvl, what_style=\"nvl_text\")");

            // Style for narrative text
            await m_Writer.WriteLineAsync("    style.nvl_text = Style(style.nvl_dialogue)");
            await m_Writer.WriteLineAsync("    style.nvl_text.size = 16");
            await m_Writer.WriteLineAsync("    style.nvl_text.line_spacing = 5");
            await m_Writer.WriteLineAsync("    style.nvl_text.color = \"#FFFFFF\"");
            await m_Writer.WriteLineAsync("    style.nvl_text.font = \"fonts/Inter.ttf\"");
            await m_Writer.WriteLineAsync("    style.nvl_text.xalign = 0.5");  // Center text horizontally
            await m_Writer.WriteLineAsync("    style.nvl_text.text_align = 0.5");  // Center text alignment
            await m_Writer.WriteLineAsync("    style.nvl_window.xfill = True");  // Fill available width
            await m_Writer.WriteLineAsync("    style.nvl_window.xmaximum = 1200");  // Set max width
            await m_Writer.WriteLineAsync();

            foreach (var character in m_Characters)
            {
                var safeName = character.Name.ToLower().Replace(" ", "_");
                await m_Writer.WriteLineAsync($"    {safeName} = Character(\"{character.Name}\", ");
                await m_Writer.WriteLineAsync($"        kind=adv, ");
                await m_Writer.WriteLineAsync($"        who_style=\"character_name\", ");
                await m_Writer.WriteLineAsync($"        what_style=\"character_dialogue\")");
            }

            // Character name and dialogue styles
            await m_Writer.WriteLineAsync("    style.character_name = Style(style.say_dialogue)");
            await m_Writer.WriteLineAsync("    style.character_name.size = 28");
            await m_Writer.WriteLineAsync("    style.character_name.bold = True");
            await m_Writer.WriteLineAsync("    style.character_name.color = \"#FFD700\"  # Gold color");
            await m_Writer.WriteLineAsync();

            await m_Writer.WriteLineAsync("    style.character_dialogue = Style(style.say_dialogue)");
            await m_Writer.WriteLineAsync("    style.character_dialogue.size = 24");
            await m_Writer.WriteLineAsync("    style.character_dialogue.color = \"#FFFFFF\"");
            await m_Writer.WriteLineAsync();
        }

        /// <summary>
        /// Writes image definitions for characters and scenes to the Ren'Py script
        /// </summary>
        private async Task WriteImageDefinitions()
        {
            await m_Writer.WriteLineAsync("# Image definitions");
            await m_Writer.WriteLineAsync("init python:");
            await m_Writer.WriteLineAsync("    # Character images");
            foreach (var character in m_Characters)
            {
                var safeName = character.Name.ToLower().Replace(" ", "_");
                await m_Writer.WriteLineAsync($"    renpy.image(\"{safeName}\", \"{Path.Combine("images", "characters", $"{safeName}.png").Replace("\\", "/")}\")");
            }

            await m_Writer.WriteLineAsync();
            await m_Writer.WriteLineAsync("    # Scene backgrounds");
            foreach (var scene in m_Scenes)
            {
                await m_Writer.WriteLineAsync($"    renpy.image(\"bg scene{scene.Id}\", \"{Path.Combine("images", "scenes", $"scene{scene.Id}.jpg").Replace("\\", "/")}\")");
            }
            await m_Writer.WriteLineAsync();
        }

        /// <summary>
        /// Writes the main story content to the Ren'Py script
        /// </summary>
        private async Task WriteStory()
        {
            await m_Writer.WriteLineAsync("# The story starts here");
            await m_Writer.WriteLineAsync("label start:");
            await m_Writer.WriteLineAsync("    $ previous_choices = {}");
            await m_Writer.WriteLineAsync("    stop music fadeout 1.0");
            await m_Writer.WriteLineAsync("    play music \"audio/background_music.mp3\" fadein 2.0 loop");
            await m_Writer.WriteLineAsync();

            foreach (var scene in m_Scenes)
            {
                await WriteScene(m_Scenes.IndexOf(scene));
            }
        }

        /// <summary>
        /// Writes a single scene to the Ren'Py script with improved transitions
        /// </summary>
        /// <param name="sceneIndex">Index of the scene in the scenes list</param>
        private async Task WriteScene(int sceneIndex)
        {
            var scene = m_Scenes[sceneIndex];

            string sceneLabel = GenerateSceneLabel(scene);
            await m_Writer.WriteLineAsync($"    # Scene {scene.Id} - {sceneLabel}");
            await m_Writer.WriteLineAsync($"label {sceneLabel}:");

            // Handle previous choices context
            if (scene.PreviousChoices.Any())
            {
                await WriteChoiceContext(scene);
            }

            // Scene setup with transition
            await m_Writer.WriteLineAsync($"    scene bg scene{scene.Id}");
            await m_Writer.WriteLineAsync("    with fade");
            await m_Writer.WriteLineAsync();

            await WriteNarrative(scene);
            await WriteChoices(sceneIndex, scene, sceneLabel);
        }

        /// <summary>
        /// Generates a unique label for the scene based on its ID and previous choices
        /// </summary>
        private string GenerateSceneLabel(Scene scene)
        {
            string choicePath = string.Join("_", scene.PreviousChoices
                .OrderBy(kv => kv.Key)
                .Select(kv => $"c{kv.Key}_{kv.Value}"));

            return string.IsNullOrEmpty(choicePath)
                ? $"scene_{scene.Id}"
                : $"scene_{scene.Id}_{choicePath}";
        }

        /// <summary>
        /// Writes the choice context variables for a scene
        /// </summary>
        private async Task WriteChoiceContext(Scene scene)
        {
            await m_Writer.WriteLineAsync("    # Previous choices context");
            foreach (var choice in scene.PreviousChoices)
            {
                await m_Writer.WriteLineAsync($"    $ choice_{choice.Key} = previous_choices.get({choice.Key}, -1)");
            }
            await m_Writer.WriteLineAsync();
        }

        /// <summary>
        /// Writes the narrative text of a scene with improved formatting and tokenized output
        /// </summary>
        private async Task WriteNarrative(Scene scene)
        {
            if (string.IsNullOrWhiteSpace(scene.Narrative))
                return;

            var paragraphs = scene.Narrative.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(para => para.Trim())
                .Where(para => !string.IsNullOrWhiteSpace(para))
                .ToList(); // Force materialization to avoid deferred execution issues

            await m_Writer.WriteLineAsync("    # Narrative text");

            foreach (var para in paragraphs)
            {
                var tokens = Regex.Split(para, @"(?<=[\.!?])\s+")
                    .Select(token => token.Trim())
                    .Where(token => !string.IsNullOrWhiteSpace(token));

                foreach (var token in tokens)
                {
                    var escapedToken = EscapeRenpyText(token);
                    await m_Writer.WriteLineAsync($"    narrator \"{escapedToken}\"");
                    await m_Writer.WriteLineAsync("    pause 0.3");
                    await m_Writer.WriteLineAsync("    nvl clear");
                }

                await m_Writer.WriteLineAsync("    pause 0.5");
            }

            await m_Writer.WriteLineAsync("    nvl clear");
            await m_Writer.WriteLineAsync();
        }

        /// <summary>
        /// Escapes text for Ren'Py script
        /// </summary>
        private string EscapeRenpyText(string text)
        {
            return text
                .Replace("\"", "\\\"")  // Escape double quotes
                .Replace("\n", " ")     // Replace newlines with spaces
                .Replace("\r", "")      // Remove carriage returns
                .Replace("[", "[[")      // Escape square brackets
                .Replace("]", "]]");
        }

        /// <summary>
        /// Writes the choices menu for a scene with improved formatting
        /// </summary>
        private async Task WriteChoices(int sceneIndex, Scene scene, string currentSceneLabel)
        {
            if (scene.Options?.Any() ?? false)
            {
                await m_Writer.WriteLineAsync("    # Player choices");
                await m_Writer.WriteLineAsync("    menu:");

                foreach (var option in scene.Options.Select((text, index) => new { text, index }))
                {
                    var escapedOption = EscapeRenpyText(option.text);
                    await m_Writer.WriteLineAsync($"        \"{escapedOption}\":");
                    await WriteChoiceAction(sceneIndex, scene, option.index);
                }
            }
            else if (scene.Depth < m_MaxDepth)
            {
                await m_Writer.WriteLineAsync($"    jump end_{sceneIndex + 2}");
            }
            else
            {
                await m_Writer.WriteLineAsync("    jump end_1");
            }
            await m_Writer.WriteLineAsync();
        }

        /// <summary>
        /// Writes the action for a specific choice
        /// </summary>
        private async Task WriteChoiceAction(int sceneIndex, Scene scene, int choiceIndex)
        {
            var nextScene = FindNextScene(scene, choiceIndex);

            if (nextScene != null)
            {
                string nextSceneLabel = GenerateSceneLabel(nextScene);
                await m_Writer.WriteLineAsync($"            $ previous_choices[{scene.Id}] = {choiceIndex}");
                await m_Writer.WriteLineAsync($"            jump {nextSceneLabel}");
            }
            else if (scene.Depth == m_MaxDepth)
            {
                await m_Writer.WriteLineAsync($"            jump end_{choiceIndex + 1}");
            }
            else
            {
                await m_Writer.WriteLineAsync($"            jump end_{choiceIndex + 1}");
            }
        }

        /// <summary>
        /// Finds the next scene based on current scene and choice
        /// </summary>
        private Scene FindNextScene(Scene currentScene, int choiceIndex)
        {
            return m_Scenes.FirstOrDefault(s =>
                s.PreviousChoices.ContainsKey(currentScene.Id) &&
                s.PreviousChoices[currentScene.Id] == choiceIndex);
        }

        /// <summary>
        /// Writes the ending labels with improved formatting
        /// </summary>
        private async Task WriteEndings()
        {
            int numEndings = m_Scenes.Count > 0 && m_Scenes.Last().Options?.Count > 0
                ? m_Scenes.Last().Options.Count
                : 1;

            await m_Writer.WriteLineAsync("# Game endings");
            for (int i = 0; i < numEndings; i++)
            {
                await m_Writer.WriteLineAsync($"label end_{i + 1}:");
                await m_Writer.WriteLineAsync($"    scene black with fade");
                await m_Writer.WriteLineAsync($"    narrator \"You reached ending #{i + 1} of your journey.\"");
                await m_Writer.WriteLineAsync($"    narrator \"Thank you for playing!\"");
                await m_Writer.WriteLineAsync("    return");
                await m_Writer.WriteLineAsync();
            }
        }
    }
}
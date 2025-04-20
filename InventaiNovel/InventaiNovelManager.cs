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
            m_Characters = m_CharacterManager.GenerateCharacters(pRequest.CharacterCreationRequest);
            m_GeneralContext = pRequest.Prompt + " with context: " + pRequest.Context;
            
            Console.WriteLine($"Creating {pRequest.NumScenes} scenes for the novel with characters: {string.Join(", ", m_Characters.Select(c => c.Name))}");
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
            Console.WriteLine($"Creating next scene for characters: {string.Join(", ", pCharacters.Select(c => c.Name))}");
            
            var optionsString = m_TextAgent.CompleteMessage(
                $"Create the options to choose for the player for the scene with the characters: {string.Join(", ", m_Characters.Select(c => c.Name))} " +
                $"with the general context: {m_GeneralContext} " +
                $"and having the metadata: {string.Join(", ", m_Characters.Select(c => c.MetaData), m_GeneralContext)}");
            
            List<string>? options = optionsString.Split("\n").ToList();

            var scene = new Scene
            {
                Id = m_Scenes.Count + 1,
                Characters = pCharacters,
                BackgroundImage = m_ImageAgent.GenerateCharacterImageAsync(
                    $"Create an image for the scene with the characters: {string.Join(", ", m_Characters.Select(c => c.Name))} " +
                    $"with the general context: {m_GeneralContext} " +
                    $"and having the metadata: {string.Join(", ", m_Characters.Select(c => c.MetaData))}", 
                    m_GeneralContext).Result,
                MetaData = m_Characters.Select(c => c.MetaData).ToList(),
                Narrative = m_TextAgent.CompleteMessage(
                    $"Create the narrative for the scene with the characters: {string.Join(", ", m_Characters.Select(c => c.Name))} " +
                    $"with the general context: {m_GeneralContext} " +
                    $"and having the metadata: {string.Join(", ", m_Characters.Select(c => c.MetaData), m_GeneralContext)}"),
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
            try
            {
                Console.WriteLine($"Starting to save novel to: {pBasePath}");
                
                // Create main novel directory
                var novelDir = Path.Combine(pBasePath, "novel");
                Directory.CreateDirectory(novelDir);
                Console.WriteLine($"Created main novel directory: {novelDir}");

                // Save characters
                await SaveCharacters(novelDir);

                // Save scenes
                await SaveScenes(novelDir);

                // Save novel metadata
                await SaveNovelMetadata(novelDir);

                Console.WriteLine("Novel saved successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving novel: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private async Task SaveCharacters(string novelDir)
        {
            var charactersDir = Path.Combine(novelDir, "characters");
            Directory.CreateDirectory(charactersDir);
            Console.WriteLine($"Created characters directory: {charactersDir}");

            foreach (var character in m_Characters)
            {
                try
                {
                    var characterImagePath = Path.Combine(charactersDir, $"{character.Name}.jpg");
                    await File.WriteAllBytesAsync(characterImagePath, character.Image);
                    Console.WriteLine($"Saved character image: {characterImagePath}");
                    
                    // Update character image path
                    character.Image = Encoding.UTF8.GetBytes($"characters/{character.Name}.jpg");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving character {character.Name}: {ex.Message}");
                    throw;
                }
            }
        }

        private async Task SaveScenes(string novelDir)
        {
            try
            {
                Console.WriteLine("Starting to save scenes...");
                var scenesDir = Path.Combine(novelDir, "scenes");
                Console.WriteLine($"Creating scenes directory at: {scenesDir}");
                Directory.CreateDirectory(scenesDir);
                Console.WriteLine($"Created scenes directory: {scenesDir}");

                Console.WriteLine($"Number of scenes to save: {m_Scenes.Count}");
                foreach (var scene in m_Scenes)
                {
                    try
                    {
                        Console.WriteLine($"Processing scene {scene.Id}...");
                        var sceneImagePath = Path.Combine(scenesDir, $"scene{scene.Id}.jpg");
                        Console.WriteLine($"Saving scene image to: {sceneImagePath}");
                        
                        if (scene.BackgroundImage == null || scene.BackgroundImage.Length == 0)
                        {
                            Console.WriteLine($"Warning: Scene {scene.Id} has no background image data");
                            continue;
                        }

                        await File.WriteAllBytesAsync(sceneImagePath, scene.BackgroundImage);
                        Console.WriteLine($"Successfully saved scene image: {sceneImagePath}");
                        
                        // Update scene image path
                        scene.BackgroundImage = Encoding.UTF8.GetBytes($"scenes/scene{scene.Id}.jpg");
                        Console.WriteLine($"Updated scene {scene.Id} image path");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing scene {scene.Id}: {ex.Message}");
                        Console.WriteLine($"Stack trace: {ex.StackTrace}");
                        throw;
                    }
                }
                Console.WriteLine("Finished saving all scenes");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SaveScenes: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private async Task SaveNovelMetadata(string novelDir)
        {
            try
            {
                Console.WriteLine("Starting to save novel metadata...");
                var novelPath = Path.Combine(novelDir, "novel.json");
                Console.WriteLine($"Preparing to save novel metadata to: {novelPath}");

                Console.WriteLine("Preparing serialization options...");
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.Preserve,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                Console.WriteLine("Preparing novel data structure...");
                var novelData = new
                {
                    Title = m_GeneralContext,
                    Characters = m_Characters.Select(c => new
                    {
                        c.Name,
                        c.MetaData,
                        ImagePath = Encoding.UTF8.GetString(c.Image)
                    }).ToList(),
                    Scenes = m_Scenes.Select(s => new
                    {
                        s.Id,
                        Narrative = s.Narrative.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(para => para.Trim())
                            .Where(para => !string.IsNullOrWhiteSpace(para))
                            .ToList(),
                        s.Options,
                        s.MetaData,
                        BackgroundImagePath = Encoding.UTF8.GetString(s.BackgroundImage),
                        CharacterNames = s.Characters.Select(c => c.Name).ToList()
                    }).ToList(),
                    CreatedAt = DateTime.UtcNow
                };

                Console.WriteLine("Serializing novel data...");
                var jsonContent = System.Text.Json.JsonSerializer.Serialize(novelData, options);
                Console.WriteLine($"Serialized data length: {jsonContent.Length} characters");
                
                Console.WriteLine($"Writing JSON to file: {novelPath}");
                await File.WriteAllTextAsync(novelPath, jsonContent);
                Console.WriteLine($"Successfully saved novel metadata to: {novelPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SaveNovelMetadata: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// Exports the novel to Ren'Py format
        /// </summary>
        /// <param name="pBasePath">Base path where to save the Ren'Py files</param>
        public async Task ExportToRenpy(string pBasePath)
        {
            try
            {
                Console.WriteLine("Starting Ren'Py export...");
                var renpyDir = Path.Combine(pBasePath, "renpy");
                Directory.CreateDirectory(renpyDir);
                Console.WriteLine($"Created Ren'Py directory: {renpyDir}");

                // Create game directory (required by Ren'Py)
                var gameDir = Path.Combine(renpyDir, "game");
                Directory.CreateDirectory(gameDir);
                Console.WriteLine($"Created game directory: {gameDir}");

                // Create images directory
                var imagesDir = Path.Combine(gameDir, "images");
                Directory.CreateDirectory(imagesDir);
                Console.WriteLine($"Created images directory: {imagesDir}");

                // Copy character images
                var charactersDir = Path.Combine(imagesDir, "characters");
                Directory.CreateDirectory(charactersDir);
                foreach (var character in m_Characters)
                {
                    var safeName = character.Name.ToLower().Replace(" ", "_");
                    var sourcePath = Path.Combine(pBasePath, "novel", "characters", $"{character.Name}.jpg");
                    var destPath = Path.Combine(charactersDir, $"{safeName}.jpg");
                    File.Copy(sourcePath, destPath, true);
                    Console.WriteLine($"Copied character image: {character.Name} -> {safeName}.jpg");
                }

                // Copy scene images
                var scenesDir = Path.Combine(imagesDir, "scenes");
                Directory.CreateDirectory(scenesDir);
                foreach (var scene in m_Scenes)
                {
                    var sourcePath = Path.Combine(pBasePath, "novel", "scenes", $"scene{scene.Id}.jpg");
                    var destPath = Path.Combine(scenesDir, $"scene{scene.Id}.jpg");
                    File.Copy(sourcePath, destPath, true);
                    Console.WriteLine($"Copied scene image: {scene.Id}");
                }

                // Generate script.rpy
                var scriptPath = Path.Combine(gameDir, "script.rpy");
                using (var writer = new StreamWriter(scriptPath))
                {
                    // Write character definitions
                    await writer.WriteLineAsync("# Character definitions");
                    await writer.WriteLineAsync("init python:");
                    await writer.WriteLineAsync("    narrator = Character(None, kind=nvl)");
                    foreach (var character in m_Characters)
                    {
                        var safeName = character.Name.ToLower().Replace(" ", "_");
                        await writer.WriteLineAsync($"    {safeName} = Character(\"{character.Name}\", kind=adv)");
                    }
                    await writer.WriteLineAsync();

                    // Write image definitions
                    await writer.WriteLineAsync("# Image definitions");
                    foreach (var character in m_Characters)
                    {
                        var safeName = character.Name.ToLower().Replace(" ", "_");
                        await writer.WriteLineAsync($"image {safeName} = \"{Path.Combine("images", "characters", $"{safeName}.jpg").Replace("\\", "/")}\"");
                    }
                    foreach (var scene in m_Scenes)
                    {
                        await writer.WriteLineAsync($"image scene{scene.Id} = \"{Path.Combine("images", "scenes", $"scene{scene.Id}.jpg").Replace("\\", "/")}\"");
                    }
                    await writer.WriteLineAsync();

                    // Write the story
                    await writer.WriteLineAsync("# The story starts here");
                    await writer.WriteLineAsync("label start:");
                    await writer.WriteLineAsync();

                    foreach (var scene in m_Scenes)
                    {
                        await writer.WriteLineAsync($"    # Scene {scene.Id}");
                        await writer.WriteLineAsync($"    scene scene{scene.Id}");
                        await writer.WriteLineAsync("    with fade");
                        await writer.WriteLineAsync();

                        // Write narrative paragraphs
                        var paragraphs = scene.Narrative.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(para => para.Trim())
                            .Where(para => !string.IsNullOrWhiteSpace(para));
                        
                        foreach (var para in paragraphs)
                        {
                            await writer.WriteLineAsync($"    narrator \"{para}\"");
                            await writer.WriteLineAsync("    nvl clear");
                        }
                        await writer.WriteLineAsync();

                        // Write choices
                        if (scene.Options != null && scene.Options.Any())
                        {
                            await writer.WriteLineAsync("    menu:");
                            for (int i = 0; i < scene.Options.Count; i++)
                            {
                                var option = scene.Options[i];
                                await writer.WriteLineAsync($"        \"{option}\":");
                                await writer.WriteLineAsync($"            jump choice{scene.Id}_{i + 1}");
                            }
                            await writer.WriteLineAsync();

                            // Add choice labels
                            for (int i = 0; i < scene.Options.Count; i++)
                            {
                                await writer.WriteLineAsync($"label choice{scene.Id}_{i + 1}:");
                                await writer.WriteLineAsync($"    narrator \"You chose option {i + 1}.\"");
                                await writer.WriteLineAsync("    jump end");
                                await writer.WriteLineAsync();
                            }
                        }
                        await writer.WriteLineAsync();
                    }

                    // Add ending
                    await writer.WriteLineAsync("label end:");
                    await writer.WriteLineAsync("    \"The End\"");
                    await writer.WriteLineAsync("    return");
                }

                Console.WriteLine($"Successfully exported Ren'Py files to: {renpyDir}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting to Ren'Py: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}

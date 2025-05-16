using Inventai.Character;
using Inventai.Core;
using Inventai.Core.Character;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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
        /// Max Scenes Depth
        /// </summary>
        private int m_MaxDepth;

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
        public async Task CreateNovel(NovelCreationRequest pRequest)
        {
            m_Characters = m_CharacterManager.GenerateCharacters(pRequest.CharacterCreationRequest);
            m_GeneralContext = pRequest.Prompt + " with context: " + pRequest.Context;
            m_MaxDepth = pRequest.NumScenes;
            
            Console.WriteLine($"Creating novel with {pRequest.NumScenes} scenes and characters: {string.Join(", ", m_Characters.Select(c => c.Name))}");
            
            // Create scenes in a branching structure
            CreateBranchingScenes(pRequest.NumScenes);
            await GenerateScenesBackgrounds();
        }

        /// <summary>
        /// Creates scenes in a branching structure
        /// </summary>
        private void CreateBranchingScenes(int totalScenes)
        {
            // Create the first scene (root of the tree)
            var storyContext = new Dictionary<int, int>();  // Tracks the story path
            CreateNextScene(m_Characters, storyContext, 1);

            // For each level (depth) in the story
            for (int depth = 2; depth <= totalScenes; depth++)
            {
                var currentLevelScenes = m_Scenes.Where(s => s.Depth == depth - 1).ToList();
                
                foreach (var currentScene in currentLevelScenes)
                {
                    // Get the number of branches (choices) for this scene
                    int numChoices = currentScene.Options?.Count ?? 0;
                    
                    // For each choice in the current scene
                    for (int choiceIndex = 0; choiceIndex < numChoices; choiceIndex++)
                    {
                        // Create a new story context that includes this choice
                        var newContext = new Dictionary<int, int>(currentScene.PreviousChoices)
                        {
                            [currentScene.Id] = choiceIndex
                        };

                        // Create the next scene with this context
                        CreateNextScene(m_Characters, newContext, depth);
                    }
                }
            }

            // Connect scenes by updating NextSceneIds
            foreach (var scene in m_Scenes)
            {
                if (scene.Options != null && scene.Options.Any())
                {
                    scene.NextSceneIds = new List<int>();
                    for (int choiceIndex = 0; choiceIndex < scene.Options.Count; choiceIndex++)
                    {
                        // Find the scene that follows this choice
                        var nextScene = m_Scenes.FirstOrDefault(s =>
                            s.PreviousChoices.ContainsKey(scene.Id) &&
                            s.PreviousChoices[scene.Id] == choiceIndex);

                        scene.NextSceneIds.Add(nextScene?.Id ?? -1);
                    }
                }
            }
        }

        /// <summary>
        /// Create the next scene
        /// </summary>
        /// <param name="pCharacters">Characters in the scene</param>
        /// <param name="previousChoices">Dictionary of previous scene IDs and chosen options</param>
        /// <param name="depth">Current depth in the story tree</param>
        private void CreateNextScene(List<Character> pCharacters, Dictionary<int, int> previousChoices, int depth)
        {
            try
            {
                if (pCharacters == null || !pCharacters.Any())
                {
                    throw new ArgumentException("Characters list cannot be null or empty");
                }

                Console.WriteLine($"Creating scene at depth {depth} with context: {string.Join(", ", previousChoices?.Select(kv => $"Scene{kv.Key}:Choice{kv.Value}") ?? Array.Empty<string>())}");
                
                // Build context based on previous choices
                var contextBuilder = new StringBuilder();
                contextBuilder.AppendLine(m_GeneralContext ?? "No general context available.");
                contextBuilder.AppendLine("\nStory progression:");

                if (previousChoices?.Any() == true)
                {
                    foreach (var choice in previousChoices.OrderBy(kv => kv.Key))
                    {
                        var previousScene = m_Scenes.FirstOrDefault(s => s.Id == choice.Key);
                        if (previousScene?.Options != null && 
                            choice.Value >= 0 && 
                            choice.Value < previousScene.Options.Count)
                        {
                            contextBuilder.AppendLine($"In the previous scene, the choice was: {previousScene.Options[choice.Value]}");
                        }
                    }
                }
                else
                {
                    contextBuilder.AppendLine("This is the beginning of the story.");
                }
                
                var sceneContext = contextBuilder.ToString().Trim();
                
                // Generate options based on the context and depth
                var optionsPrompt = $"Create {(depth < m_Scenes.Count ? "2-3" : "2-4")} meaningful choices for scene {depth} considering:\n" +
                                  $"Characters: {string.Join(", ", pCharacters.Select(c => c.Name ?? "Unknown"))}\n" +
                                  $"Story context: {sceneContext}\n" +
                                  $"This is {(depth < m_Scenes.Count ? "not" : "")} the final scene.\n" +
                                  $"Each choice should lead to a distinctly different story progression." + 
                                  $"The choices should be in the format:\n" +
                                    $"1. Choice 1\n" +
                                    $"2. Choice 2\n" +
                                    $"3. Choice 3\n" +
                                    $"4. Choice 4\n" +
                                    $"Ensure the choices are clear, engaging and does not exceed one sentence each.\n";

                var optionsString = m_TextAgent.CompleteMessage(optionsPrompt);
                List<string> options = (optionsString ?? string.Empty)
                    .Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(o => o.Trim())
                    .Where(o => !string.IsNullOrWhiteSpace(o))
                    .Take(4)  // Limit maximum choices
                    .ToList();

                // Ensure we have at least one option
                if (!options.Any())
                {
                    options.Add("Continue the story...");
                }

                // Generate narrative based on the context
                var narrativePrompt = $"Create a narrative for scene {depth} considering:\n" +
                                    $"Characters: {string.Join(", ", pCharacters.Select(c => c.Name ?? "Unknown"))}\n" +
                                    $"Story context: {sceneContext}\n" +
                                    $"Available choices: {string.Join(", ", options)}\n" +
                                    $"Make the narrative reflect the consequences of previous choices and lead naturally to the available options." +
                                    $"The narrative should be engaging and not exceed 3-5 short sentences.\n" +
                                    $"Format the response as a single string without any additional formatting or JSON." +
                                    $"Ensure the response is valid and contains a coherent narrative." +
                                    $"The narrative should not contain the choices themselves." +
                                    $"The narrative should be split into sentences and not exceed 200 characters.";

                var narrative = m_TextAgent.CompleteMessage(narrativePrompt);
                if (string.IsNullOrWhiteSpace(narrative))
                {
                    narrative = "The story continues...";
                }

                var imagePrompt = "";
                try
                {
                    // Extract key elements from the narrative for the image
                    var narrativeSummary = m_TextAgent.CompleteMessage($"Summarize the following narrative in a few key elements for an image prompt:\n{narrative}" +
                                    $"\n\nKey elements: " +
                                    $"1. Characters: {string.Join(", ", pCharacters.Select(c => c.Name ?? "Unknown"))}\n" +
                                    $"2. Scene description: {narrative} " +
                                    $"3. Mood/Theme: {string.Join(", ", options)}\n" +
                                    $"4. Setting: {sceneContext}" +
                                    $"\n\nUse these elements to create a concise image prompt." +
                                    $"Ensure the response is valid and contains a coherent summary that should not exceed 200 characters." +
                                    $"Format the response as a single string without any additional formatting or JSON." +
                                    $"Ensure the response is valid and contains a coherent summary.");

                    // Create a concise, focused image prompt
                    imagePrompt = $"A detailed illustration of a scene where {string.Join(" and ", pCharacters.Select(c => c.Name ?? "Unknown"))} " +
                                    $"are in a scene. {narrativeSummary}";

                    // Ensure the prompt isn't too long
                    if (imagePrompt.Length > 200)
                    {
                        imagePrompt = imagePrompt.Substring(0, 200) + "...";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Failed to generate scene image: {ex.Message}");
                    // Continue without image - the Scene class should handle null BackgroundImage
                }

                var scene = new Scene
                {
                        Id = Scene.SceneIdCounter++,
                        Depth = depth,
                        Characters = new List<Character>(pCharacters),  // Create a new list to avoid reference issues
                        BackgroundImage = [],
                        BgImagePrompt = imagePrompt,
                        MetaData = pCharacters.Select(c => c.MetaData ?? "No metadata").ToList(),
                        Narrative = narrative,
                        Options = options,
                        PreviousChoices = new Dictionary<int, int>(previousChoices ?? new Dictionary<int, int>()),
                        SceneContext = sceneContext,
                        NextSceneIds = new List<int>()  // Will be populated later
                };

                // If it is the last scene, set the EndingTales
                if (depth == m_MaxDepth)
                {
                    foreach (var sceneOption in options)
                    {
                        var endingTale = m_TextAgent.CompleteMessage($"Create an ending tale for the scene {depth} with the following option: {sceneOption}" +
                        $"The context is: {sceneContext}" +
                        $"The narrative is: {narrative}" +
                        $"The characters are: {string.Join(", ", pCharacters.Select(c => c.Name ?? "Unknown"))}" +
                        $"The available choices are: {string.Join(", ", options)}" +
                        $"The ending tale should be engaging and not exceed 3-5 short sentences." +
                        $"Format the response as a single string without any additional formatting or JSON." +
                        $"Ensure the response is valid and contains a coherent ending tale." +
                        $"The ending tale should not contain the choices themselves." +
                        $"The ending tale should be split into sentences and not exceed 200 characters.");
                        if (!string.IsNullOrWhiteSpace(endingTale))
                        {
                            scene.EndingTales = scene.EndingTales.Append(endingTale).ToArray();
                        }
                        else
                        {
                            scene.EndingTales = scene.EndingTales.Append("The story concludes...").ToArray();
                        }
                    }
                }
                else
                {
                    scene.EndingTales = Array.Empty<string>();
                }

                m_Scenes.Add(scene);
                Console.WriteLine($"Created scene {depth} with {options.Count} choices");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating scene at depth {depth}: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// Generates scenes backgrounds
        /// </summary>
        /// <returns></returns>
        private async Task GenerateScenesBackgrounds()
        {
            List<Task<byte[]>> imageGenerationsTasks = [];
            m_Scenes.ForEach(scene =>
            {
                imageGenerationsTasks.Add(m_ImageAgent.GenerateImageAsync(new ImageRequest()
                {
                    Prompt = scene.BgImagePrompt,
                    NegativePrompt = "text, watermark, signature, blurry, distorted",
                    Width = 1024,
                    Height = 728,
                }));
            });

            byte[][] images = await Task.WhenAll(imageGenerationsTasks);

            for (int i = 0; i < m_Scenes.Count; i++)
            {
                m_Scenes[i].BackgroundImage = images[i];
            }
        }

        /// <summary>
        /// Loads a novel from a file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Novel LoadNovel(string path)
        {
            try
            {
                Console.WriteLine($"Loading novel from: {path}");
                var json = File.ReadAllText(path);
                var novel = JsonSerializer.Deserialize<Novel>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReferenceHandler = ReferenceHandler.Preserve
                });

                if (novel == null)
                {
                    throw new Exception("Failed to deserialize the novel");
                }
                m_Characters = novel.Characters;
                m_Scenes = novel.Scenes;

                Console.WriteLine("Novel loaded successfully!");
                return novel;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading novel: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// Returns the characters of the novel
        /// </summary>
        /// <returns></returns>
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

        #region Assets 
        private async Task CopyAudios(string pBasePath)
        {
            // var inventaiNovelResourcesDir is the current directory of this before the build
            var inventaiNovelAudioDir = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
                "Resources", "audio");
            var audioFiles = Directory.GetFiles(inventaiNovelAudioDir, "*.*", SearchOption.AllDirectories)
                .Where(file => file.EndsWith(".mp3") || file.EndsWith(".wav"))
                .ToList();

            if (!audioFiles.Any())
            {
                Console.WriteLine("No audio files found to copy.");
                return;
            }
            foreach (var file in audioFiles)
            {
                var fileName = Path.GetFileName(file);
                var destPath = Path.Combine(pBasePath, fileName);
                File.Copy(file, destPath);
                Console.WriteLine($"Copied audio file: {fileName}");
            }
        }

        private async Task CopyFonts(string pBasePath)
        {
            // var inventaiNovelResourcesDir is the current directory of this before the build
            var inventaiNovelFontsDir = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
                "Resources", "fonts");
            var fontFiles = Directory.GetFiles(inventaiNovelFontsDir, "*.*", SearchOption.AllDirectories)
                .Where(file => file.EndsWith(".ttf") || file.EndsWith(".otf"))
                .ToList();
            if (!fontFiles.Any())
            {
                Console.WriteLine("No font files found to copy.");
                return;
            }
            foreach (var file in fontFiles)
            {
                var fileName = Path.GetFileName(file);
                var destPath = Path.Combine(pBasePath, fileName);
                File.Copy(file, destPath);
                Console.WriteLine($"Copied font file: {fileName}");
            }
        }
        #endregion

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

                // Copy audio files
                var audioDir = Path.Combine(gameDir, "audio");
                Directory.CreateDirectory(audioDir);
                await CopyAudios(audioDir);
                Console.WriteLine($"Created audio directory: {audioDir}");
                // Copy font files
                var fontsDir = Path.Combine(gameDir, "fonts");
                Directory.CreateDirectory(fontsDir);
                await CopyFonts(fontsDir);
                Console.WriteLine($"Created fonts directory: {fontsDir}");

                // Copy character images
                var charactersDir = Path.Combine(imagesDir, "characters");
                Directory.CreateDirectory(charactersDir);

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
                    var exporter = new RenpyExporter(m_Scenes, m_Characters, m_GeneralContext, m_MaxDepth, writer);
                    await exporter.ExportToRenpy();
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

using Inventai.Core;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http;
using System.Threading.Tasks;
using System;

namespace Inventai.ImageAgents
{
    public class SnakeCasePropertyNamesContractResolver : DefaultContractResolver
    {
        public SnakeCasePropertyNamesContractResolver()
        {
            NamingStrategy = new SnakeCaseNamingStrategy();
        }
    }

    /// <summary>
    /// Stable diffusion Image agent
    /// </summary>
    public class ImageAgentStableDiffusion : IImageAgent
    {
        /// <summary>
        /// Http Client of the agent
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Stable diffusion API endpoint
        /// </summary>
        private readonly string _endpoint;

        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings()
        {
            ContractResolver = new SnakeCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
        };

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="pEndpoint"></param>
        public ImageAgentStableDiffusion(string pEndpoint, string pApiKey)
        {
            _httpClient = new HttpClient();
            // Add X-API-Key to request headers
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-API-Key", pApiKey);
            _endpoint = pEndpoint;
            Console.WriteLine($"Stable Diffusion Image agent created with endpoint {pEndpoint}");
        }

        public Task<byte[]> GenerateCharacterImageAsync(string pPrompt, string pContext)
        {
            return GenerateImageAsync(new ImageRequest()
            {
                Prompt = pPrompt + " with context: " + pContext
            });
        }

        public async Task<byte[]> GenerateImageAsync(ImageRequest pRequest)
        {
            try
            {
                Console.WriteLine("Generating image for the prompt: " + pRequest.Prompt + " with negative prompt: " + pRequest.NegativePrompt);
                var response = await _httpClient.PostAsync(_endpoint,
                    new StringContent(JsonConvert.SerializeObject(pRequest, _settings), Encoding.UTF8, "application/json"));

                response.EnsureSuccessStatusCode();
                // Read the image as a byte array
                var imageData = await response.Content.ReadAsByteArrayAsync();
                Console.WriteLine("Image generated successfully");
                return imageData;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error generating image: " + e.Message);
                return new byte[0];
            }
        }
    }
}
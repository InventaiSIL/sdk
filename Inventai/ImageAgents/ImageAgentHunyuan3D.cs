using Inventai.Core;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using System.Collections.Generic;

namespace Inventai.ImageAgents;

/// <summary>
/// Hunyuan3D-2 Image Agent (Local API)
/// </summary>
public class ImageAgentHunyuan3D
{
    private readonly HttpClient _httpClient;
    private readonly string _endpoint;
    private readonly JsonSerializerSettings _settings = new()
    {
        ContractResolver = new SnakeCasePropertyNamesContractResolver(),
        Formatting = Formatting.Indented
    };

    /// <summary>
    /// Constructor for Hunyuan3D-2 Image Agent
    /// </summary>
    public ImageAgentHunyuan3D(string endpoint)
    {
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(15)
        };
        _endpoint = endpoint;
    }

    /// <summary>
    /// Generates a 3D model using text or an image
    /// </summary>
    public async Task<byte[]> Generate3DModelAsync(Hunyuan3DRequest request)
    {
        if (string.IsNullOrEmpty(request.Prompt) && string.IsNullOrEmpty(request.ImageBase64))
            throw new ArgumentException("❌ ERROR: Either 'Prompt' or 'ImageBase64' must be provided.");

        // Create request payload
        var requestBody = new Dictionary<string, object>();

        if (!string.IsNullOrEmpty(request.Prompt))
            requestBody["text"] = request.Prompt;

        if (!string.IsNullOrEmpty(request.ImageBase64))
            requestBody["image"] = request.ImageBase64;

        // Convert request to JSON
        string jsonPayload = JsonConvert.SerializeObject(requestBody, _settings);

        Console.WriteLine($"📡 Sending Request to Hunyuan3D-2 API:\n{jsonPayload}\n");

        var jsonContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        // Send POST request to Hunyuan3D-2 API
        var response = await _httpClient.PostAsync(_endpoint, jsonContent);
        response.EnsureSuccessStatusCode();

        // Return the generated 3D model data
        Console.WriteLine("✅ Hunyuan3D-2 API Response: 3D model generated successfully.");
        return await response.Content.ReadAsByteArrayAsync();
    }

    
}

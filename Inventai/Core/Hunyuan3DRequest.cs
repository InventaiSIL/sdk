/*namespace Inventai.Core;

/// <summary>
/// Request object for Hunyuan3D-2 API
/// </summary>
public class Hunyuan3DRequest
{
    /// <summary>
    /// Optional text prompt to generate a 3D model
    /// </summary>
    public string? Prompt { get; set; }

    /// <summary>
    /// Optional base64-encoded image for generating a 3D model
    /// </summary>
    public string? ImageBase64 { get; set; }

    /// <summary>
    /// Converts an image file to a base64 string
    /// </summary>
    public static async Task<string> ConvertImageToBase64Async(string imagePath)
    {
        if (!File.Exists(imagePath))
            throw new FileNotFoundException("Input image not found!", imagePath);

        byte[] imageBytes = await File.ReadAllBytesAsync(imagePath);
        return Convert.ToBase64String(imageBytes);
    }
}*/

namespace Inventai.Core;
using System;
using System.IO;
using System.Threading.Tasks;

/// <summary>
/// Request object for Hunyuan3D-2 API
/// </summary>
public class Hunyuan3DRequest
{
    /// <summary>
    /// Optional text prompt to generate a 3D model
    /// </summary>
    public string? Prompt { get; set; }

    /// <summary>
    /// Optional base64-encoded image for generating a 3D model
    /// </summary>
    public string? ImageBase64 { get; set; }

    /// <summary>
    /// Converts an image file to a base64 string
    /// </summary>
    public static async Task<string> ConvertImageToBase64Async(string imagePath)
    {
        if (!File.Exists(imagePath))
            throw new FileNotFoundException($"❌ ERROR: Input image not found! ({imagePath})");

        byte[] imageBytes = await File.ReadAllBytesAsync(imagePath);
        string base64String = Convert.ToBase64String(imageBytes);

        Console.WriteLine($"✅ Image successfully converted to Base64. Length: {base64String.Length} characters.");
        return base64String;
    }
}

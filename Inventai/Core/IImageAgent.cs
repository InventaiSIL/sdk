using System.Threading.Tasks;

namespace Inventai.Core
{

    /// <summary>
    /// Image request object
    /// </summary>
    public class ImageRequest
    {
        public string Prompt { get; set; }
        public string NegativePrompt { get; set; } = "";
        public int Width { get; set; } = 1024;
        public int Height { get; set; } = 512;
        public int Steps { get; set; } = 50;
    }

    /// <summary>
    /// Image response object
    /// </summary>
    public class ImageResponse
    {
        public string ImageUrl { get; set; }
        public byte[] ImageData { get; set; }
    }


    /// <summary>
    /// Base interface to be implemented by the text agents
    /// </summary>
    public interface IImageAgent
    {
        /// <summary>
        /// Generates an image asynchroneously from a text <paramref name="pRequest"/>
        /// </summary>
        /// <param name="pRequest"></param>
        /// <returns></returns>
        Task<byte[]> GenerateImageAsync(ImageRequest pRequest);

        /// <summary>
        /// Generates an image for a character asynchroneously from a prompt <paramref name="pPrompt"/> and a context <paramref name="pContext"/>
        /// </summary>
        /// <param name="pPrompt"></param>
        /// <param name="pContext"></param>
        /// <returns></returns>
        Task<byte[]> GenerateCharacterImageAsync(string pPrompt, string pContext);
    }
}

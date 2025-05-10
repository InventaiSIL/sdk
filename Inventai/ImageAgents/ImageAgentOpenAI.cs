using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Inventai.ImageAgents
{
    public static class ImageGeneration
    {
        public class ImageGenerationResult
        {
            public string ImageBase64 { get; set; }
            public string RawResponse { get; set; }
        }

        public class ImageGenerationApiResponse
        {
            public List<ImageData> data { get; set; }
        }
        public class ImageData
        {
            public string b64_json { get; set; }
        }

        public static async Task<ImageGenerationResult> GenerateImageAsync(string prompt, string apiKey, string modelId, string baseUrl)
        {
            using (var client = new HttpClient())
            {
                object requestBody;
                if (modelId == "gpt-image-1")
                {
                    requestBody = new
                    {
                        prompt = prompt,
                        model = modelId,
                        background = "transparent",
                    };
                }
                else
                {
                    requestBody = new
                    {
                        prompt = prompt,
                        model = modelId,
                        response_format = "b64_json"
                    };
                }

                string json = JsonConvert.SerializeObject(requestBody);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(baseUrl, content);
                response.EnsureSuccessStatusCode();

                var responseData = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    Logger.WriteError($"Request failed: {response.StatusCode}\nResponse body: {responseData}");
                    throw new HttpRequestException($"Request failed: {response.StatusCode}");
                }
                var responseJson = JsonConvert.DeserializeObject<ImageGenerationApiResponse>(responseData);
                string imageBase64 = responseJson.data[0].b64_json;

                return new ImageGenerationResult
                {
                    ImageBase64 = imageBase64,
                    RawResponse = responseData
                };
            }
        }

        public static async Task<ImageGenerationResult> EditImageWithGptAsync(string imagePath, string prompt, string apiKey, string baseUrl)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new System.Exception("API Key is missing in Inventai settings.");
            if (string.IsNullOrEmpty(imagePath) || !System.IO.File.Exists(imagePath))
                throw new System.Exception("Image file does not exist: " + imagePath);

            using (var client = new HttpClient())
            using (var form = new MultipartFormDataContent())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

                form.Add(new StringContent("gpt-image-1"), "model");
                form.Add(new StringContent(prompt), "prompt");
                form.Add(new StringContent("transparent"), "background");

                byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);
                var imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                form.Add(imageContent, "image[]", System.IO.Path.GetFileName(imagePath));

                string editsUrl = baseUrl.Replace("generations", "edits");

                HttpResponseMessage response = await client.PostAsync(editsUrl, form);
                string responseData = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Request failed: {response.StatusCode}\nResponse body: {responseData}");
                }

                var responseJson = JsonConvert.DeserializeObject<ImageGenerationApiResponse>(responseData);
                string imageBase64 = responseJson.data[0].b64_json;

                return new ImageGenerationResult
                {
                    ImageBase64 = imageBase64,
                    RawResponse = responseData
                };
            }
        }
    }
}


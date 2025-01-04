using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using TextToSpeech.Models;
using Newtonsoft.Json;
using TextToSpeech.Services;
using TextToSpeech.Middlewares;
using DotNetEnv;

namespace TextToSpeech.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TextParserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiKey, _ttsUrl;

        public TextParserController(IConfiguration config)
        {
            Env.Load();
            _configuration = config;
            _apiKey = Environment.GetEnvironmentVariable("API_KEY") ?? "";
            _ttsUrl = Environment.GetEnvironmentVariable("TTS_URL") ?? "";
        }

        [HttpPost("tts-ai")]
        public async Task<IActionResult> GetTextToSpeech([FromBody] TTSRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Input))
                {
                    return BadRequest(new { Message = "Input text is required." });
                }

                var wordCount = request.Input.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
                if (wordCount > 40)
                {
                    return BadRequest(new { Message = "Input text exceeds the 40-word limit. Please shorten your text." });
                }

                if (string.IsNullOrWhiteSpace(_apiKey))
                {
                    return StatusCode(500, new { Message = "OpenAI API key is missing or not configured." });
                }

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var payload = new
                    {
                        model = "tts-1",
                        input = request.Input,
                        voice = request.Voice ?? "alloy"
                    };

                    var jsonPayload = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(_ttsUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var audioData = await response.Content.ReadAsByteArrayAsync();

                        var modifiedAudioData = ModifyAudioData(audioData);

                        var audioFile = CreateFormFile(modifiedAudioData, "modified_speech.mp3", "audio/mpeg");

                        var analyzer = new AudioAnalyzer(_configuration);
                        AudioAnalysisResponse result = await analyzer.GetAudioAsync(audioFile);

                        var dataCollector = new DataCollector(new RealtimeDatabaseService(_configuration));
                        await dataCollector.CollectDataAsync(result, request.Voice ?? "alloy");

                        return File(audioData, "audio/mpeg", "speech.mp3");
                    }
                    else
                    {
                        var errorDetails = await response.Content.ReadAsStringAsync();
                        return StatusCode((int)response.StatusCode, new
                        {
                            Message = "Error from OpenAI API",
                            Details = errorDetails
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while processing the TTS request",
                    Details = ex.Message
                });
            }
        }

        private byte[] ModifyAudioData(byte[] originalAudioData)
        {
            byte[] header = Encoding.UTF8.GetBytes("DummyHeader");
            byte[] modifiedData = new byte[header.Length + originalAudioData.Length];

            Buffer.BlockCopy(header, 0, modifiedData, 0, header.Length);
            Buffer.BlockCopy(originalAudioData, 0, modifiedData, header.Length, originalAudioData.Length);

            return modifiedData;
        }

        private IFormFile CreateFormFile(byte[] data, string fileName, string contentType)
        {
            var stream = new MemoryStream(data);
            return new FormFile(stream, 0, data.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
        }
    }
}

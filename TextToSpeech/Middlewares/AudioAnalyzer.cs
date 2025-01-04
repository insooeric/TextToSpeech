using TextToSpeech.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using DotNetEnv;

namespace TextToSpeech.Middlewares
{
    public class AudioAnalyzer
    {
        private readonly string _apiKey, _sstUrl;

        public AudioAnalyzer(IConfiguration config)
        {
            Env.Load();
            _apiKey = Environment.GetEnvironmentVariable("API_KEY") ?? "";
            _sstUrl = Environment.GetEnvironmentVariable("STT_URL") ?? "";
        }

        public async Task<AudioAnalysisResponse> GetAudioAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new ArgumentException("Audio file is required.");
                }

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

                    using (var content = new MultipartFormDataContent())
                    {
                        var fileStream = file.OpenReadStream();
                        var fileContent = new StreamContent(fileStream);
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
                        content.Add(fileContent, "file", file.FileName);

                        content.Add(new StringContent("whisper-1"), "model");
                        content.Add(new StringContent("verbose_json"), "response_format");

                        var response = await client.PostAsync(_sstUrl, content);

                        await fileStream.DisposeAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            var responseData = await response.Content.ReadAsStringAsync();
                            TranscriptionResponse? data = JsonConvert.DeserializeObject<TranscriptionResponse>(responseData);

                            if (data == null)
                            {
                                throw new Exception("Failed to parse audio analysis response.");
                            }

                            List<Sentence> sentences = new List<Sentence>();

                            foreach (Segment segment in data.Segments)
                            {
                                Sentence sentence = new Sentence()
                                {
                                    Id = segment.Id,
                                    Duration = segment.End - segment.Start,
                                };
                                sentences.Add(sentence);
                            }

                            AudioAnalysisResponse audioAnalysisResponse = new AudioAnalysisResponse()
                            {
                                Language = data.Language,
                                Duration = data.Duration,
                                Sentences = sentences
                            };

                            if(audioAnalysisResponse.Language == "")
                            {
                                throw new Exception($"Error from API: Language not found");
                            }

                            return audioAnalysisResponse;
                        }
                        else
                        {
                            var errorDetails = await response.Content.ReadAsStringAsync();
                            throw new Exception($"Error from API: {errorDetails}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                throw new Exception($"An error occurred while analyzing the audio: {ex.Message}");
            }
        }
    }
}

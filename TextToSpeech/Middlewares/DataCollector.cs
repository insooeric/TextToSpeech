using TextToSpeech.Models;
using TextToSpeech.Services;

namespace TextToSpeech.Middlewares
{
    public class DataCollector
    {
        private readonly RealtimeDatabaseService _databaseService;

        public DataCollector(RealtimeDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task CollectDataAsync(AudioAnalysisResponse response, string toneType)
        {
            if (response == null || string.IsNullOrWhiteSpace(response.Language))
                throw new ArgumentException("Invalid audio analysis response.");

            string language = response.Language;

            await UpdateLanguageStatisticsAsync(response, toneType);
            await UpdateTotalStatisticsAsync(response, toneType);
        }

        private async Task UpdateLanguageStatisticsAsync(AudioAnalysisResponse response, string toneType)
        {
            string languagePath = $"statistics/languages/{response.Language}";

            var languageStats = await _databaseService.GetDataAsync<LanguageStatistics>(languagePath) ?? new LanguageStatistics();

            languageStats.LangTotalRequest++;
            languageStats.LangTotalDuration += response.Duration;
            languageStats.LangAverageDuration = languageStats.LangTotalDuration / languageStats.LangTotalRequest;
            languageStats.LangTotalSentences += response.Sentences?.Count ?? 0;
            languageStats.LangAverageDurationPerSentences = languageStats.LangTotalDuration / languageStats.LangTotalSentences;

            if (languageStats.LangToneTypeCounts.ContainsKey(toneType))
            {
                languageStats.LangToneTypeCounts[toneType]++;
            }
            else
            {
                languageStats.LangToneTypeCounts[toneType] = 1;
            }

            await _databaseService.PutDataAsync(languagePath, languageStats);
        }

        private async Task UpdateTotalStatisticsAsync(AudioAnalysisResponse response, string toneType)
        {
            string totalPath = "statistics/total";

            var totalStats = await _databaseService.GetDataAsync<TotalStatistics>(totalPath) ?? new TotalStatistics();

            totalStats.GlobalTotalRequests++;
            totalStats.GlobalTotalDuration += response.Duration;
            totalStats.GlobalTotalAverageDuration = totalStats.GlobalTotalDuration / totalStats.GlobalTotalRequests;
            totalStats.GlobalTotalSentences += response.Sentences?.Count ?? 0;
            totalStats.GlobalAverageDurationPerSentences = totalStats.GlobalTotalDuration / totalStats.GlobalTotalSentences;

            if (totalStats.GlobalToneTypeCounts.ContainsKey(toneType))
            {
                totalStats.GlobalToneTypeCounts[toneType]++;
            }
            else
            {
                totalStats.GlobalToneTypeCounts[toneType] = 1;
            }

            await _databaseService.PutDataAsync(totalPath, totalStats);
        }
    }
}

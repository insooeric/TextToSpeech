namespace TextToSpeech.Models
{
    public class LanguageStatisticsContainer
    {
        public Dictionary<string, LanguageStatistics> languages { get; set; } = new Dictionary<string, LanguageStatistics>();
    }
    public class LanguageStatistics
    {
        public int LangTotalRequest { get; set; }
        public double LangTotalDuration { get; set; }
        public double LangAverageDuration { get; set; }
        public int LangTotalSentences { get; set; }
        public double LangAverageDurationPerSentences { get; set; }

        public Dictionary<string, int> LangToneTypeCounts { get; set; } = new Dictionary<string, int>();
    }
}

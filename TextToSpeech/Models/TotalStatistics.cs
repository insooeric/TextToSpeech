namespace TextToSpeech.Models
{
    public class TotalStatistics
    {
        public int GlobalTotalRequests { get; set; }
        public double GlobalTotalDuration { get; set; }
        public double GlobalTotalAverageDuration { get; set; }
        public int GlobalTotalSentences { get; set; }
        public double GlobalAverageDurationPerSentences { get; set; }
        public Dictionary<string, int> GlobalToneTypeCounts { get; set; } = new Dictionary<string, int>();
    }
}

namespace TextToSpeech.Models
{
    public class AudioAnalysisResponse
    {
        public string Language { get; set; } = "";
        public double Duration { get; set; }
        public List<Sentence> Sentences { get; set; } = new List<Sentence>();
    }

    public class Sentence
    {
        public int Id { get; set; }
        public double Duration { get; set; }
    }
}

namespace TesseractWrappers.Models
{
    public interface IRecognitionResult
    {
        public string Text { get; }
        public float MeanConfidence { get; }
    }
}
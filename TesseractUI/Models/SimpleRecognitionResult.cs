namespace TesseractUI.Models
{
    public class SimpleRecognitionResult : IRecognitionResult
    {
        public string Text { get; }
        public float MeanConfidence { get; }

        public SimpleRecognitionResult(string text, float meanConfidence)
        {
            this.Text = text;
            this.MeanConfidence = meanConfidence;
        }
    }
}
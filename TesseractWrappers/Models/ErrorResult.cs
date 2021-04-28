using System;

namespace TesseractWrappers.Models
{
    public class ErrorResult : IRecognitionResult
    {
        public Exception Exception { get; set; }
        
        public ErrorResult(Exception exception)
        {
            this.Exception = exception;
        }

        public string Text => this.Exception.ToString();
        public float MeanConfidence => 0f;
    }
}
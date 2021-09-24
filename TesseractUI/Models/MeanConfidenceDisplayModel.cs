using System;
using System.Globalization;
using System.Windows.Media;

namespace TesseractUI.Models
{
    public class MeanConfidenceDisplayModel
    {
        public string TextValue { get; set; } = String.Empty;
        public Brush Background { get; set; } =  new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        
        public MeanConfidenceDisplayModel()
        {
        }
        
        public MeanConfidenceDisplayModel(float value)
        {
            Color color = Color.FromArgb(100, (byte) (255 * (1 - value)), (byte) (255 * value), 0);
            this.TextValue = value.ToString(CultureInfo.InvariantCulture);
            this.Background = new SolidColorBrush(color);
        }
        
        public static implicit operator MeanConfidenceDisplayModel(float f)
        {
            return new MeanConfidenceDisplayModel(f);
        }
    }
}
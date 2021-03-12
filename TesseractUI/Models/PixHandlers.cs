using Tesseract;

namespace TesseractUI.Models
{
    public interface IHandler<T>
    {
        public string Name { get; } 
        
        IHandler<T> SetNext(IHandler<T> handler);
        
        T Handle(T request);
    }

    public abstract class PixHandler : IHandler<Pix>
    {
        public int Id { get; set; }
        
        private IHandler<Pix> _nextHandler;

        public string Name { get; } = "DefaultHandler";

        public IHandler<Pix> SetNext(IHandler<Pix> handler)
        {
            this._nextHandler = handler;
            
            return handler;
        }
        
        public virtual Pix Handle(Pix request)
        {
            if (_nextHandler!=null)
            {
               return _nextHandler.Handle(request);
            }
            return request;
        }
    }

    public class GrayscaleHandler : PixHandler
    {
        public string Name { get; } = "Convert To Grayscale";
        
        public override Pix Handle(Pix request)
        {
            request = request?.ConvertRGBToGray();
            return base.Handle(request);
        }
    }

    public class DeskewHandler : PixHandler
    {
        public string Name { get; } = "Deskew";
        
        public override Pix Handle(Pix request)
        {
            request = request?.Deskew();
            return base.Handle(request);
        }
    }
    
    public class DespeckleHandler : PixHandler
    {
        public string Name { get; } = "Despeckle";
        
        /// <summary>
        /// hit-miss sels in 2D layout; SEL_STR2 and SEL_STR3 are predefined values
        /// </summary>
        public HitMissSels SelStr { get; set; }
        
        /// <summary>
        /// 2 for 2x2, 3 for 3x3
        /// </summary>
        public int SelSize { get; set; }
        
        public DespeckleHandler(HitMissSels selStr, int selSize)
        {
            this.SelStr = selStr;
            this.SelSize = selSize;
        }
        
        public enum HitMissSels
        {
            SEL_STR2,
            SEL_STR3
        }
        
        public override Pix Handle(Pix request)
        {
            request = request?.Despeckle(this.SelStr.ToString(), this.SelSize);
            return base.Handle(request);
        }
    }

    public class ScaleHandler : PixHandler
    {
        public string Name { get; } = "Scale Image";
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        
        public ScaleHandler(float scaleX, float scaleY)
        {
            this.ScaleX = scaleX;
            this.ScaleY = scaleY;
        }
        
        public override Pix Handle(Pix request)
        {
            request = request?.Scale(this.ScaleX, this.ScaleY);
            return base.Handle(request);
        }
    }

    public class BinarizeSauvolaHandler : PixHandler
    {
        public string Name { get; set; } = "Binarize by Sauvola method";

        /// <summary>
        /// the window half-width for measuring local statistics
        /// </summary>
        public int WindowHalfWidth { get; set; }
        /// <summary>
        /// The factor for reducing threshold due to variances greater than or equal to zero (0). Typically around 0.35.
        /// </summary>
        public float ReduceFactor { get; set; }
        /// <summary>
        /// If <c>True</c> add a border of width (<ref name="WindowHalfWidth" /> + 1) on all sides.
        /// </summary>
        public bool AddBorder { get; set; }
        
        public BinarizeSauvolaHandler(int windowHalfWidth, float reduceFactor = 0.35f, bool addBorder = false)
        {
            this.WindowHalfWidth = windowHalfWidth;
            this.ReduceFactor = reduceFactor;
            this.AddBorder = addBorder;
        }

        public override Pix Handle(Pix request)
        {
            request = request?.BinarizeSauvola(this.WindowHalfWidth, this.ReduceFactor, this.AddBorder);
            return base.Handle(request);
        }
    }

    public class BinarizeOtsu : PixHandler
    {
        public string Name { get; set; } = "Binarize by Otsu method";
        
        /// <summary>
        /// sizeX Desired tile X dimension; actual size may vary.
        /// </summary>
        public int SizeX { get; set; }
        /// <summary>
        /// sizeY Desired tile Y dimension; actual size may vary
        /// </summary>
        public int SizeY { get; set; }
        /// <summary>
        /// smoothX Half-width of convolution kernel applied to threshold array: use 0 for no smoothing.
        /// </summary>
        public int SmoothX { get; set; }
        /// <summary>
        /// smoothY Half-height of convolution kernel applied to threshold array: use 0 for no smoothing.
        /// </summary>
        public int SmoothY { get; set; }
        /// <summary>
        /// scoreFraction Fraction of the max Otsu score; typ. 0.1 (use 0.0 for standard Otsu)
        /// </summary>
        public float ScoreFraction { get; set; }
        
        
        public BinarizeOtsu(int sizeX, int sizeY, int smoothX = 0, int smoothY = 0, float scoreFraction = 0.1f)
        {
            this.SizeX = sizeX;
            this.SizeY = sizeY;
            this.SmoothX = smoothX;
            this.SmoothY = smoothY;
            this.ScoreFraction = scoreFraction;
        }
        
        public override Pix Handle(Pix request)
        {
            request = request?.BinarizeOtsuAdaptiveThreshold(this.SizeX, this.SizeY, this.SmoothX,this.SmoothY, this.ScoreFraction);
            return base.Handle(request);
        }
    }

    public class RemoveLinesHandler : PixHandler
    {
        public string Name { get; } = "Remove Lines";
        
        public override Pix Handle(Pix request)
        {
            request = request?.RemoveLines();
            return base.Handle(request);
        }
    }
    
}
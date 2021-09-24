using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Tesseract;
using TesseractUI.Annotations;
using TesseractUI.Helpers;
using TesseractUI.Models;
using TesseractWrappers;
using TesseractWrappers.Models;

namespace TesseractUI
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public const string RESULT_FILE_NAME = "temp.png";

        private Pix _pix;
        
        private EngineMode _selectedEngineMode = EngineMode.Default;
        private readonly PixHandlersManager _manager = new PixHandlersManager();
        private string _decodedText;
        private MeanConfidenceDisplayModel _meanConfidence = new MeanConfidenceDisplayModel();
        private ImageSource _baseImage;
        private ImageSource _resultImage;
        private string _baseImageFilePath;
        private string _baseImageSizeText;
        
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
        
        public EngineMode SelectedEngineMode
        {
            get => _selectedEngineMode;
            set
            {
                if (value == _selectedEngineMode) return;
                _selectedEngineMode = value;
                OnPropertyChanged();
            }
        }

        public string BaseImageFilePath
        {
            get => _baseImageFilePath;
            set
            {
                if (value == _baseImageFilePath) return;
                _baseImageFilePath = value;
                OnPropertyChanged();
            }
        }

        public string DecodedText
        {
            get => _decodedText;
            set
            {
                if (value == _decodedText) return;
                _decodedText = value;
                OnPropertyChanged();
            }
        }
        
        public MeanConfidenceDisplayModel MeanConfidence
        {
            get => _meanConfidence;
            set
            {
                if (Equals(value, _meanConfidence)) return;
                _meanConfidence = value;
                OnPropertyChanged();
            }
        }
        
        public ImageSource BaseImage
        {
            get => _baseImage;
            set
            {
                if (Equals(value, _baseImage)) return;
                _baseImage = value;
                OnPropertyChanged();
            }
        }
        
        public ImageSource ResultImage
        {
            get => _resultImage;
            set
            {
                if (Equals(value, _resultImage)) return;
                _resultImage = value;
                OnPropertyChanged();
            }
        }
        
        public string BaseImageSizeText
        {
            get => _baseImageSizeText;
            set
            {
                if (value == _baseImageSizeText) return;
                _baseImageSizeText = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<EngineMode> EngineModes => 
            new ObservableCollection<EngineMode>(Enum.GetValues(typeof(EngineMode)).Cast<EngineMode>());
        
        public ObservableCollection<PixHandler> Handlers => new ObservableCollection<PixHandler>(_manager.Handlers);


        public void SelectFile(string fileName)
        {
            this.BaseImageFilePath = fileName;
            this.BaseImage = ImageSourceHelper.BitmapFromFilePath(fileName);
            
            _pix = Pix.LoadFromFile(this.BaseImageFilePath);
            this.BaseImageSizeText = _pix.Height + "x" + _pix.Width;

            ClearOldResults();
            
            PreviewImage();
        }

        public void Recognize()
        {
            if (!string.IsNullOrEmpty(this.BaseImageFilePath))
            {
                // Console.WriteLine(string.Join("\r\n", Directory.EnumerateFiles("./")));
                using (TesseractEngine engine = new TesseractEngine(@"./tessdata", "rus", this.SelectedEngineMode))
                {
                    IRecognitionResult result = TessEngineWrapper.ReadFile(_pix, engine);
                    
                    this.MeanConfidence = result.MeanConfidence;
                    this.DecodedText = result.Text;
                }
            }
        }
        
        /// <summary>
        /// Create a preview
        /// </summary>
        public void PreviewImage()
        {
            Pix pix = GetHandlersChain().Handle(_pix);
            pix.Save(RESULT_FILE_NAME, ImageFormat.Default);
            this.ResultImage = ImageSourceHelper.BitmapFromFilePath(Path.GetFullPath(RESULT_FILE_NAME));
        }

        private void ClearOldResults()
        {
            Console.WriteLine("ClearOldResults");
            
            this.MeanConfidence = new MeanConfidenceDisplayModel();
            this.DecodedText = "";
            this.ResultImage = null;
            
            if (File.Exists(RESULT_FILE_NAME))
            {
                File.Delete(RESULT_FILE_NAME);
            }
        }

        private PixHandler GetHandlersChain()
        {
            PixHandler handler = this.Handlers.Aggregate((cur, next) =>
            {
                cur.SetNext(next);
                return next;
            });
            
            return handler;
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Tesseract;
using TesseractUI.Models;
using Path = System.IO.Path;

namespace TesseractUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string RESULT_FILE_NAME = "temp.png";
        public string FilePath { get; set; }
        public Pix Pix { get; set; }
        
        public EngineMode SelectedEngineMode { get; set; }
        public IEnumerable<EngineMode> EngineModes
        {
            get { return Enum.GetValues(typeof(EngineMode)).Cast<EngineMode>(); }
        }

        public MainWindow()
        {
            InitializeComponent();
            
            // DecodedText.Text = "Естественно, желательно примерно представлять, какие языки могут встречаться в документе. Чем больше языков используется — тем дольше работает распознавание. " +
            //                    "Иногда Tesseract некорректно обрабатывает случаи, когда текст на разных языках встречается рядом в одной строке. В таких случаях попробуйте ранее перечисленные способы по улучшению качества распознавания. Если не поможет, то попробуйте обходной путь — распознавайте отдельные слова на разных языках и в каждом случае выбирайте результат с большим значением confidence. Пример кода:";
        }

        private void SelectButton_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Image Files|*.bmp;*.png;*.jpg;*.tif;|All files (*.*)|*.*";
            
            if (openFileDialog.ShowDialog() == true)
            {
                this.FilePath = openFileDialog.FileName;
                FileName.Text = this.FilePath;
                MainImage.Source = BitmapFromUri(new Uri(this.FilePath));
                
                this.Pix = Pix.LoadFromFile(this.FilePath);
                SetMainImageSize(this.Pix);
                
                ClearOldValues();

                PreviewImage();
            }
        }

        /// <summary>
        /// Create a prieview
        /// </summary>
        private void PreviewImage()
        {
            this.Pix = GetHandlers().Handle(this.Pix);
            this.Pix.Save(RESULT_FILE_NAME, ImageFormat.Default);
            ResultImage.Source = BitmapFromUri(new Uri(Path.GetFullPath(RESULT_FILE_NAME)));
        }

        private PixHandler GetHandlers()
        {
            DeskewHandler deskewHandler = new DeskewHandler();
            GrayscaleHandler grayscaleHandler = new GrayscaleHandler();
            deskewHandler.SetNext(grayscaleHandler);

            return deskewHandler;
        }

        private void ClearOldValues()
        {
            Console.WriteLine("ClearOldValues");
            ClearMeanConfidence();
            DecodedText.Text = "";
            ResultImage.Source = null;
            
            if (File.Exists(RESULT_FILE_NAME))
            {
                File.Delete(RESULT_FILE_NAME);
            }
        }

        private void RecognizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.FilePath))
            {
                // Console.WriteLine(string.Join("\r\n", Directory.EnumerateFiles("./")));
                using (TesseractEngine engine = new TesseractEngine(@"./tessdata", "rus", EngineMode.TesseractAndLstm))
                {
                    IRecognitionResult result = TessEngineWrapper.ReadFile(this.Pix, engine);

                    SetMeanConfidence(result.MeanConfidence);
                    DecodedText.Text = result.Text;
                }
            }
        }

        private void SetMainImageSize(Pix pix)
        {
            string picSize = pix.Height + "x" + pix.Width;
            MainImageSize.Text = picSize;
        }

        private void SetMeanConfidence(float meanConf)
        {
            Color color = Color.FromArgb(100, (byte) (255 * (1 - meanConf)), (byte) (255 * meanConf), 0);
            MeanConfidence.Text = meanConf.ToString(CultureInfo.InvariantCulture);
            MeanConfidence.Background = new SolidColorBrush(color);
        }

        private void ClearMeanConfidence()
        {
            MeanConfidence.Text = "";
            MeanConfidence.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }
        
        public static ImageSource BitmapFromUri(Uri source)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = source;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }
        
    }
}
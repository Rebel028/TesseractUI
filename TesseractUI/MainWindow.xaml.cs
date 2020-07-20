using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Tesseract;
using Page =Tesseract.Page;
using Path = System.IO.Path;

namespace TesseractUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string FilePath { get; set; }
        private const string TEMP_FILE_NAME = "temp.png";
        
        public MainWindow()
        {
            InitializeComponent();
            DecodedText.Text = "Естественно, желательно примерно представлять, какие языки могут встречаться в документе. Чем больше языков используется — тем дольше работает распознавание. " +
                               "Иногда Tesseract некорректно обрабатывает случаи, когда текст на разных языках встречается рядом в одной строке. В таких случаях попробуйте ранее перечисленные способы по улучшению качества распознавания. Если не поможет, то попробуйте обходной путь — распознавайте отдельные слова на разных языках и в каждом случае выбирайте результат с большим значением confidence. Пример кода:";
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
                MainImage.Source = new BitmapImage(new Uri(this.FilePath));
                ResultImage.Source = null;
                DecodedText.Text = "";
            }
        }

        private void RecognizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.FilePath))
            {
                // Console.WriteLine(string.Join("\r\n", Directory.EnumerateFiles("./")));
                using (TesseractEngine engine = new TesseractEngine(@"./tessdata", "rus", EngineMode.LstmOnly))
                {
                    ReadFile(this.FilePath, engine);
                }
            }
        }
        
        private void ReadFile(string file, TesseractEngine engine)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            Pix img = null;

            try
            {
                img = Pix.LoadFromFile(file);
                img = img.ConvertRGBToGray();
                img = img.Deskew(out Scew skew);

                
                img.Save(TEMP_FILE_NAME, ImageFormat.Default);
                
                using (Page page = engine.Process(img))
                {
                    string text = page.GetText();
                    DecodedText.Text = $"Mean confidence: {page.GetMeanConfidence()}\r\n" +
                                       $"Text: {text}";
                    ResultImage.Source = new BitmapImage(new Uri(Path.GetFullPath(TEMP_FILE_NAME)));
#if USE_ITER
                            IterateBlocks(page);
#endif
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                timer.Stop();
                Console.WriteLine("Processing time: " + timer.Elapsed);
                if (img!=null && !img.IsDisposed)
                {
                    img.Dispose();
                }
            }
        }

        private static void IterateBlocks(Page page)
        {
            Console.WriteLine("Text (iterator):");

            using (ResultIterator iter = page.GetIterator())
            {
                iter.Begin();

                do
                {
                    do
                    {
                        do
                        {
                            do
                            {
                                if (iter.IsAtBeginningOf(PageIteratorLevel.Block))
                                {
                                    Console.WriteLine("<BLOCK>");
                                }

                                Console.Write(iter.GetText(PageIteratorLevel.Word));
                                Console.Write(" ");

                                if (iter.IsAtFinalOf(PageIteratorLevel.TextLine, PageIteratorLevel.Word))
                                {
                                    Console.WriteLine();
                                }
                            }
                            while (iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));

                            if (iter.IsAtFinalOf(PageIteratorLevel.Para, PageIteratorLevel.TextLine))
                            {
                                Console.WriteLine();
                            }
                        }
                        while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
                    }
                    while (iter.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));
                }
                while (iter.Next(PageIteratorLevel.Block));
            }
        }
    }
}
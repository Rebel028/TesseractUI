using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Tesseract;
using TesseractWrappers;
using TesseractWrappers.Models;
using WPF.JoshSmith.ServiceProviders.UI;
using Path = System.IO.Path;

namespace TesseractUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string RESULT_FILE_NAME = "temp.png";
        //public string FilePath { get; set; }
        public Pix Pix { get; set; }

        public EngineMode SelectedEngineMode { get; set; } = EngineMode.Default;
        public IEnumerable<EngineMode> EngineModes => Enum.GetValues(typeof(EngineMode)).Cast<EngineMode>();

        private ListViewDragDropManager<PixHandler> _listViewManager;

        private readonly PixHandlersManager _manager = new PixHandlersManager();

        public List<PixHandler> Handlers => _manager.Handlers;
        
        private MainViewModel Model => (MainViewModel)this.DataContext;
        
        public MainWindow()
        {
            InitializeComponent();
            _listViewManager = new ListViewDragDropManager<PixHandler>(this.HandlersListView);
            
            // EngineModeCombobox.ItemsSource = this.EngineModes;
            //
            // HandlersListView.ItemsSource = this.Handlers;
            
            // ReSharper disable CommentTypo
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
                string selectedFileName = openFileDialog.FileName;
                this.Model.SelectFile(selectedFileName);
            }
        }

        private void RecognizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            RecognizeButton.IsEnabled = false;
            Task.Run(this.Model.Recognize).Wait();
            RecognizeButton.IsEnabled = true;
        }

        private void AddHandlerButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void EditHandler_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void RemoveHandler_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
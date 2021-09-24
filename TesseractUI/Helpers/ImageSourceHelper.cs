using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TesseractUI.Helpers
{
    public static class ImageSourceHelper
    {

        public static ImageSource BitmapFromFilePath(string file)
        {
            return BitmapFromUri(new Uri(file));
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
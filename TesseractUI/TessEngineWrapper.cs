using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;
using Tesseract;
using TesseractUI.Models;

namespace TesseractUI
{
    public static class TessEngineWrapper
    {

        public static IRecognitionResult ReadFile(Pix img ,TesseractEngine engine)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            
            try
            {
                using (Page page = engine.Process(img))
                {
                    string text = page.GetText();
                    float meanConf = page.GetMeanConfidence();
                    return new SimpleRecognitionResult(text, meanConf);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ErrorResult(e);
            }
            finally
            {
                timer.Stop();
                Console.WriteLine("Processing time: " + timer.Elapsed);
                bool imageIsNotDisposed = img!=null && !img.IsDisposed;
                Console.WriteLine("imageIsNotDisposed " + imageIsNotDisposed);
                if (imageIsNotDisposed)
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
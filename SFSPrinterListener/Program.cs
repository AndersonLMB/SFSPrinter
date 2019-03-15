using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Net;
using System.Drawing.Printing;
using System.IO;
using System.Drawing;
using WebSocketSharp.Server;
using System.Windows.Forms;

namespace SFSPrinterListener
{
    class Program
    {
        static void Main(string[] args)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add(String.Format("http://{1}:{0}/", 5757, "localhost"));
            while (true)
            {
                listener.Start();
                var context = listener.GetContext();
                var request = context.Request;
                var response = context.Response;
                var rawUrl = request.RawUrl;
                var a = rawUrl.Split('/');
                try
                {
                    if (a.Length > 1)
                    {
                        var operationAndKvps = a[1];
                        var operation = operationAndKvps.Split('?')[0];
                        var kvps = operationAndKvps.Split('?')[1];
                        var kvpsDictionary = new Dictionary<string, string>();
                        kvps.Split('&').ToList().ForEach((kvp) =>
                        {
                            kvpsDictionary.Add(kvp.Split('=')[0], kvp.Split('=')[1]);
                        });
                        ;
                    }
                }
                catch (Exception)
                {
                    //throw;
                }


                if (request.HttpMethod.ToUpper() == "POST")
                {
                    Image img = null;
                    using (StreamReader sr = new StreamReader(request.InputStream))
                    {
                        var base64str = sr.ReadToEnd();
                        img = Util.GetImageByBase64String(base64str);


                    }

                    if (img != null)
                    {
                        var printDocument = new PrintDocument();
                        //var margins = printDocument.DefaultPageSettings.Margins;
                        //margins.Top = 1000;


                        Util.FitImageToDocument(img, printDocument, new DefaultFitSettings());
                        //printDocument.DefaultPageSettings.Landscape = true;
                        //printDocument.PrintPage += (s, e) =>
                        //{
                        //    e.Graphics.DrawImage(img, new Point(0, 0));






                        //};


                        var printPrevDlg = new PrintPreviewDialog
                        {
                            Document = printDocument
                        };

                        var result = printPrevDlg.ShowDialog();
                        switch (result)
                        {
                            case DialogResult.None:
                                break;
                            case DialogResult.OK:
                                break;
                            case DialogResult.Cancel:
                                break;
                            case DialogResult.Abort:
                                break;
                            case DialogResult.Retry:
                                break;
                            case DialogResult.Ignore:
                                break;
                            case DialogResult.Yes:
                                break;
                            case DialogResult.No:
                                break;
                            default:
                                break;
                        }




                    }
                }
                response.ContentLength64 = 0;
                response.Close();
                listener.Stop();
            }
        }

        static Bitmap GetBitmap(byte[] buf)
        {
            Int16 width = BitConverter.ToInt16(buf, 18);
            Int16 height = BitConverter.ToInt16(buf, 22);

            Bitmap bitmap = new Bitmap(width, height);

            int imageSize = width * height * 4;
            int headerSize = BitConverter.ToInt16(buf, 10);

            System.Diagnostics.Debug.Assert(imageSize == buf.Length - headerSize);

            int offset = headerSize;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bitmap.SetPixel(x, height - y - 1, Color.FromArgb(buf[offset + 3], buf[offset], buf[offset + 1], buf[offset + 2]));
                    offset += 4;
                }
            }
            return bitmap;
        }

    }

    static class Util
    {
        public static Image GetImageByBase64String(string base64string)
        {
            var data = Convert.FromBase64String(base64string);
            var ms = new MemoryStream(data);
            var img = Image.FromStream(ms);
            return img;
        }


        public static void FitImageToDocument(Image image, PrintDocument document)
        {
            document.PrintPage += (s, e) =>
            {
            };

        }

        public static void FitImageToDocument(Image image, PrintDocument document, IImageFitSettings settings)
        {
            Console.WriteLine($"{document.DefaultPageSettings.PrinterResolution.X} * {document.DefaultPageSettings.PrinterResolution.Y} ");
            //document.DefaultPageSettings.PrinterResolution.X = 300;

            Console.WriteLine($"{ document.DefaultPageSettings.PaperSize.Width} * {document.DefaultPageSettings.PaperSize.Height} ");
            document.DefaultPageSettings.Landscape = true;
            Console.WriteLine($"{ document.DefaultPageSettings.PaperSize.Width} * {document.DefaultPageSettings.PaperSize.Height} ");


            document.PrintPage += (s, e) =>
            {
                Console.WriteLine($"Resolutions of image {image.HorizontalResolution} * {image.VerticalResolution}");
                Console.WriteLine($"Size of image : {image.Size.Width}*{image.Size.Height}");
                //e.Graphics.DrawImage(image, new Point(0, 0));
                e.Graphics.DrawImage(image, new Point(0, 0));
                Console.WriteLine($"{ document.DefaultPageSettings.PaperSize.Width} * {document.DefaultPageSettings.PaperSize.Height} ");

            };

            ;
        }
    }

    public class DefaultFitSettings : IImageFitSettings
    {
        public double Left { get; set; } = 0;
        public double Bottom { get; set; } = 0;
        public double Right { get; set; } = 0;
        public double Top { get; set; } = 0;
        public bool Landscape { get; set; } = false;
    }

    public interface IImageFitSettings
    {
        double Left { get; set; }
        double Bottom { get; set; }
        double Right { get; set; }
        double Top { get; set; }
        bool Landscape { get; set; }
    }


}

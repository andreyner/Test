using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageConverter_to_ASCII
{
   public class PNG_JPG_Converter : BaseConverter
    {

        public PNG_JPG_Converter(UI ui) : base(ui)
        {

        }
        public override string  StartConverter(int startWidthPos,int quality, CancellationToken token)
        {
            try
            {
                Bitmap image = GetReSizedImage(quality);
                ImageInTxt = RunConverter(startWidthPos, token, image);
            }
            catch { }
            return ImageInTxt;

           
        }
        private string RunConverter(int startWidthPos, CancellationToken token, Bitmap image)
        {

            Boolean toggle = false;
            StringBuilder sb = new StringBuilder();
            ui.Invoke(ui.DelegateProgressBar,new object[] {image.Height});
            for (int h = 0; h < image.Height; h++)
            {
                ui.Invoke(new MethodInvoker(ui.barIncrement));
                for (int w = startWidthPos; w < image.Width; w++)
                {
                    if (token.IsCancellationRequested)
                    {
                        return sb.ToString();
                    }
                        Color pixelColor = image.GetPixel(w, h);
                    //Average out the RGB components to find the Gray Color
                    int red = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                    int green = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                    int blue = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                    Color grayColor = Color.FromArgb(red, green, blue);

                    //Use the toggle flag to minimize height-wise stretch
                    if (!toggle)
                    {
                        int index = (grayColor.R * (ui.chars.Count-1)) / 255;
                       
                        sb.Append(ui.chars[index]);
                    }
                }
                if (!toggle)
                {
                    sb.Append("<BR>");
                    toggle = true;
                }
                else
                {
                    toggle = false;
                }
            }

            return sb.ToString();
        }
        protected Bitmap GetReSizedImage(int asciiWidth)
        {
            int asciiHeight = 0;
            //Calculate the new Height of the image from its width
            asciiHeight = (int)Math.Ceiling((double)ui.Image.Height * asciiWidth / ui.Image.Width);

            //Create a new Bitmap and define its resolution
            Bitmap result = new Bitmap(asciiWidth, asciiHeight);
            Graphics g = Graphics.FromImage((Image)result);
            //The interpolation mode produces high quality images 
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(ui.Image, 0, 0, asciiWidth, asciiHeight);
            g.Dispose();
            return result;
        }

    }
}

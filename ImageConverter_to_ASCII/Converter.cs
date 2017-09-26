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
   public class Converter 
    {

        public Converter(UI ui)
        {
            this.ui = ui;
        }
        /// <summary>
        /// Изображение в виде  ASCII символов
        /// </summary>
        protected string ImageInTxt { get; set; }
        /// <summary>
        /// Интерфейс пользователя
        /// </summary>
        protected UI ui { get; set; }
        /// <summary>
        /// Запускает конвертер
        /// </summary>
        /// <param name="quality">качество изображения</param>
        /// <param name="token">признак отмены работы конвертора</param>
        /// <returns>Изображение в виде ASCII символов </returns>
        public string  StartConverter(int quality, CancellationToken token)
        {
            try
            {
                Bitmap image = GetNewImage(quality);
                ImageInTxt = RunConverter( token, image);
            }
            catch { }
            return ImageInTxt;

           
        }
        /// <summary>
        /// Непосредственно преобразование в ASCII
        /// </summary>
        /// <param name="token"> Признак отменты конвертации</param>
        /// <param name="image"> Преобразуемое изображение</param>
        /// <returns></returns>
        private string RunConverter(CancellationToken token, Bitmap image)
        {

            bool Switch = false;
            StringBuilder sb = new StringBuilder();
            ui.Invoke(ui.DelegateProgressBar,new object[] {image.Height});
            for (int h = 0; h < image.Height; h++)
            {
                ui.Invoke(new MethodInvoker(ui.barIncrement));
                if (!Switch)
                {
                    for (int w = 0; w < image.Width; w++)
                    {
                        if (token.IsCancellationRequested)
                        {
                            return sb.ToString();
                        }
                        Color pixelColor = image.GetPixel(w, h);
                        int red = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                        int green = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                        int blue = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                        Color gray = Color.FromArgb(red, green, blue);

                        int index = (gray.R * (ui.chars.Count - 1)) / 255;

                        sb.Append(ui.chars[index]);
                    }
                    sb.Append("<BR>");
                    Switch = true;
                }
                else
                {
                    Switch = false;
                }
            }

            return sb.ToString();
        }
        /// <summary>
        /// Преобразованное в размере изображение в зависимости от параметра quality
        /// </summary>
        /// <param name="quality">желаемое качество (ширина по сути)</param>
        /// <returns> преобразованное изображение</returns>
        protected Bitmap GetNewImage(int quality)
        {
            int newHeight = 0;
            newHeight = (int)Math.Ceiling((double)ui.Image.Height * quality / ui.Image.Width);
            Bitmap result = new Bitmap(quality, newHeight);
            Graphics g = Graphics.FromImage((Image)result);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(ui.Image, 0, 0, quality, newHeight);
            g.Dispose();
            return result;
        }

    }
}

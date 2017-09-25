using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageConverter_to_ASCII
{
   public abstract class BaseConverter
    {
        public BaseConverter(UI ui)
        { 
            this.ui = ui;
        }
        /// <summary>
        /// Изображение в виде символов
        /// </summary>
        protected string ImageInTxt { get; set; }
        /// <summary>
        /// Интерфейс пользователя
        /// </summary>
        protected UI ui { get; set;}
        /// <summary>
        /// Запускает конвертер
        /// </summary>
        /// <param name="startPos">начальная позиция в изображении</param>
        /// <param name="quality">качество изображения</param>
        /// <param name="token">признак отмены</param>
        /// <returns>Изображение в виде ASCII символов </returns>
        public abstract string StartConverter(int startPos, int quality,CancellationToken token);

     

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageConverter_to_ASCII
{
    public partial class UI : Form
    {

       
        public UI()
        {
            InitializeComponent();
            for (int i = 0; i < Chars.Items.Count; i++)
            {
                Chars.SetItemChecked(i, true);
            }
            DelegateProgressBar = SettingProgressBar;
        }
        public  Action<int> DelegateProgressBar;
        void SettingProgressBar(int maximum)
        {
            progressBar1.Maximum = maximum;
            
        }
     
        /// <summary>
        /// Поле для хранения изображения
        /// </summary>
        public Bitmap Image {

            get
            {
                
                    return image;
                
            }
            set
            {

                    image = value;
                    trackBar1.Maximum = image.Width;
                    if (image.Width > 50)
                    {
                        trackBar1.TickFrequency = image.Width / 50;
                        trackBar1.Value = 50;
                    }
                    else {

                        trackBar1.TickFrequency = 1;
                        trackBar1.Value = 1;
                    }
                    
                
            }
            
        }
       private Bitmap image;
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //openFileDialog1.Filter = "Image Files(*.png,*jpg,*bmp,*ico,*jpeg,*gif) | *.png;*.jpg;*.bmp*;*.ico;*.jpeg;*.gif";
                openFileDialog1.Title = "Открытие файла";
                openFileDialog1.Multiselect = false;
                openFileDialog1.FileName = "";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    image = new Bitmap(openFileDialog1.FileName, true);
                    Image = image;
                    Picture.Image = Image;

                }
                else { return; }
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Некорректный файл", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {
                MessageBox.Show("Ошибка при открытии файла", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// Уровень качества изображения
        /// </summary>
        int quality { get; set; }
        /// <summary>
        /// Коллекция выбранных ASCII символов
        /// </summary>
        public List<string> chars { get; set; }
        /// <summary>
        /// Конвертер изображений
        /// </summary>
        BaseConverter convertrer { get; set; }
        private async void button5_Click(object sender, EventArgs e)
        {
            try
            {
                
                progressBar1.Value = progressBar1.Minimum;
                tableLayoutPanel7.Enabled = false;
                btOpen.Enabled = false;
                btStart.Enabled = false;
                btStop.Enabled = true;
                menuStrip1.Enabled = false;
                convertrer = new PNG_JPG_Converter(this);
                chars = new List<string>();
                foreach (var item in Chars.CheckedItems)
                {
                    chars.Add(item.ToString());
                }
                tokenSource = new CancellationTokenSource();
                CancellationToken token = tokenSource.Token;
                Task<string>  first = Task.Run(

                    () => convertrer.StartConverter(0, quality, token)

                    );
                await first;
                if (!tokenSource.IsCancellationRequested)
                {
                   
                    txt.DocumentText = "<pre>" + "<Font size=0>" + first.Result.Replace(" ", "") + "</Font></pre>";
                    res = first.Result.Replace("<BR>", Environment.NewLine).Replace(" ", "");
  
                }
               
             
            }
            catch
            {

                MessageBox.Show("Ошибка при конвертации изображения", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                tableLayoutPanel7.Enabled = true;
                btOpen.Enabled = true;
                btStart.Enabled = true;
                btStop.Enabled = false;
                menuStrip1.Enabled = true;
            }

        }


        public void barIncrement()
        {
            progressBar1.Increment(1);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (trackBar1.Value -trackBar1.TickFrequency >= trackBar1.Minimum)
            {
                trackBar1.Value -= trackBar1.TickFrequency;
                button5_Click(sender, e);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (trackBar1.Value + trackBar1.TickFrequency <= trackBar1.Maximum)
            {
                trackBar1.Value += trackBar1.TickFrequency;
                button5_Click(sender, e);

            }
        }

        private void tableLayoutPanel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            quality = trackBar1.Value;
        }
        /// <summary>
        /// Результат преобразования
        /// </summary>
        string res;
        private void button2_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Text File (*.txt)|.txt";
            saveFileDialog1.Title = "Сохранение файла";
            try {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName))
                    {
                        sw.Write(res);
                    }
                    MessageBox.Show("Файл успешно сохранён", "Сохранение", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch
            {

                MessageBox.Show("Ошибка при сохранении файла","Ошибка",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// Признак отменты работы конвертора
        /// </summary>
        CancellationTokenSource tokenSource;
        private void button6_Click(object sender, EventArgs e)
        {
            if (tokenSource != null)
            {
                tokenSource.Cancel();
                progressBar1.Value = progressBar1.Minimum;

            }
        }

        private void UI_FormClosing(object sender, FormClosingEventArgs e)
        {
            button6_Click( sender, e);
        }
    }
}

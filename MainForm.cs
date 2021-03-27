using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormatFile
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Текстовый файл|*.txt";
            if (openFile.ShowDialog() == DialogResult.OK)
                textBox1.Text = openFile.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog saveFile = new FolderBrowserDialog();
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                if(!File.Exists(Path.Combine(saveFile.SelectedPath,
                    Path.GetFileNameWithoutExtension(textBox1.Text) + "-format.txt")) || 
                    MessageBox.Show("Файл с именем '" + Path.GetFileNameWithoutExtension(textBox1.Text) + "-format.txt'" + 
                    " уже существует.\nПерезаписать файл?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    textBox2.Text = saveFile.SelectedPath;

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("Нееобходимо указать пути файлов для чтения и записи");
                return;
            }
            StreamReader reader = new StreamReader(textBox1.Text);
            Text t = new Text(reader.ReadToEnd());
            reader.Close();

            StreamWriter writer = new StreamWriter(Path.Combine(textBox2.Text,
                    Path.GetFileNameWithoutExtension(textBox1.Text) + "-format.txt"));
            writer.Write(t.ToString());
            writer.Close();

            MessageBox.Show("Операция выполнена успешно", "Отчёт");
        }
    }
}

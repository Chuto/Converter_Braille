using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Threading;
using Converter_Braille.Models;
using Converter_Braille.Translater;

namespace Converter_Braille
{


    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        byte[] brailleUni = {
                            226,160,129,// а русский
                            226,160,131,// б
                            226,160,186,// в
                            226,160,155,// г
                            226,160,153,// д
                            226,160,145,// е
                            226,160,161,// ё
                            226,160,154,// ж
                            226,160,181,// з
                            226,160,138,// и
                            226,160,175,// й
                            226,160,133,// к
                            226,160,135,// л
                            226,160,141,// м
                            226,160,157,// н
                            226,160,149,// о
                            226,160,143,// п
                            226,160,151,// р
                            226,160,142,// с
                            226,160,158,// т
                            226,160,165,// у
                            226,160,139,// ф
                            226,160,147,// х
                            226,160,137,// ц
                            226,160,159,// ч
                            226,160,177,// ш
                            226,160,173,// щ
                            226,160,183,// ъ
                            226,160,174,// ы
                            226,160,190,// ь
                            226,160,170,// э
                            226,160,179,// ю
                            226,160,171,// я
                            226,160,129,// а
                            226,160,131,// б
                            226,160,186,// в
                            226,160,155,// г
                            226,160,153,// д
                            226,160,145,// е
                            226,160,161,// ё
                            226,160,154,// ж
                            226,160,181,// з
                            226,160,138,// и
                            226,160,175,// й
                            226,160,133,// к
                            226,160,135,// л
                            226,160,141,// м
                            226,160,157,// н
                            226,160,149,// о
                            226,160,143,// п
                            226,160,151,// р
                            226,160,142,// с
                            226,160,158,// т
                            226,160,165,// у
                            226,160,139,// ф
                            226,160,147,// х
                            226,160,137,// ц
                            226,160,159,// ч
                            226,160,177,// ш
                            226,160,173,// щ
                            226,160,183,// ъ
                            226,160,174,// ы
                            226,160,190,// ь
                            226,160,170,// э
                            226,160,179,// ю
                            226,160,171,// я
                            226,160,178,// .
                            226,160,150,// !
                            226,160,164,// -
                            226,160,166,// «
                            226,160,180,// »
                            226,160,163,// (
                            226,160,156,// )
                            226,160,130,// ,
                            226,160,162,// ?
                            226,160,128,// ' '
                            226,160,129,// A english
                            226,160,131,// B
                            226,160,137,// C
                            226,160,153,// D
                            226,160,145,// E
                            226,160,139,// F
                            226,160,155,// G
                            226,160,147,// H
                            226,160,138,// I
                            226,160,154,// J
                            226,160,133,// K
                            226,160,135,// L
                            226,160,141,// M
                            226,160,157,// N
                            226,160,149,// O
                            226,160,143,// P
                            226,160,159,// Q
                            226,160,151,// R
                            226,160,142,// S
                            226,160,158,// T
                            226,160,165,// U
                            226,160,167,// V
                            226,160,186,// W
                            226,160,173,// X
                            226,160,189,// Y
                            226,160,181,// Z
                            226,160,129,// a 
                            226,160,131,// b
                            226,160,137,// c
                            226,160,153,// d
                            226,160,145,// e
                            226,160,139,// f
                            226,160,155,// g
                            226,160,147,// h
                            226,160,138,// i
                            226,160,154,// j
                            226,160,133,// k
                            226,160,135,// l
                            226,160,141,// m
                            226,160,157,// n
                            226,160,149,// o
                            226,160,143,// p
                            226,160,159,// q
                            226,160,151,// r
                            226,160,142,// s
                            226,160,158,// t
                            226,160,165,// u
                            226,160,167,// v
                            226,160,186,// w
                            226,160,173,// x
                            226,160,189,// y
                            226,160,181,// z
                            226,160,154,// 0
                            226,160,129,// 1 
                            226,160,131,// 2
                            226,160,137,// 3
                            226,160,153,// 4
                            226,160,145,// 5
                            226,160,139,// 6
                            226,160,155,// 7
                            226,160,147,// 8
                            226,160,138,// 9
                                //20,20,13,// \n
                                  };

        int len;
        string alph = "АБВГДЁЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя.!-«»(),? ABCDEFGHIJKLNMOPQRSTUVWXYZabcdefghijklnmopqrstuvwxyz0123456789";
        OpenFileDialog inputFile = new OpenFileDialog();
        SaveFileDialog outputFile = new SaveFileDialog();
        Encoding ans = Encoding.GetEncoding(1251), uni = Encoding.Unicode;
        public MemoryStream MS = new MemoryStream();
        private BackgroundWorker backgroundWorker;
        private ManualResetEvent _workerCompleted = new ManualResetEvent(false);
        Stopwatch clock;
        static public Dictionary<char, Letter> _dictionary = new Dictionary<char, Letter>();

        string input_text;


        public MainWindow()
        {
            for (int i = 0; i < alph.Length; i++)
                _dictionary.Add(alph[i], new Letter
                {
                    s = alph[i],
                    a = ans.GetBytes(alph.ToCharArray(), i, 1),
                    u = uni.GetBytes(alph.ToCharArray(), i, 1),
                    b = Converter.Copy(brailleUni, i * 3, 3)  // ы
                });

            InitializeComponent();
            MI_Language_RU.IsChecked = true;
            MI_Save_txt.IsChecked = true;
            backgroundWorker = (BackgroundWorker)this.FindResource("backgroundWoker");
        }

        private void BackgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.Value += e.ProgressPercentage;
        }

        private void BackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            MemoryStream mmss = new MemoryStream();

            int step = input_text.Length / 100 > 0 ? 1 : 100 / input_text.Length;


            BackgroundWorker worker = sender as BackgroundWorker;
            for (int i = 0; i < len; i++)
            {
                if (_dictionary.ContainsKey(input_text[i]))
                    mmss.Write(_dictionary[input_text[i]].b, 0, 3);

                if (i % (int)((input_text.Length / 100) + 1) == 0 || step > 1)
                    worker.ReportProgress(step);
            }
            MS = mmss;
            _workerCompleted.Set();
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            //           textBox_Output.
            // textBox_Output.AppendText(Encoding.UTF8.GetString(MS.ToArray()));
            textBox_Output.Text = Encoding.UTF8.GetString(MS.ToArray());
            if (outputFile.FileName != String.Empty && outputFile.CheckPathExists)
                File.WriteAllText(outputFile.FileName, Encoding.UTF8.GetString(MS.ToArray()), Encoding.UTF8);
            MS.Close();
            progressBar1.Value = 1;
            clock.Stop();
            textBlock.Text = clock.Elapsed.ToString("mm\\:ss\\.ff");
            //button_Convert.Content = "Convert\n" + clock.Elapsed.ToString("mm\\:ss\\:ff");
            button_Convert.IsEnabled = true;
        }


        private void button_Convert_Click(object sender, RoutedEventArgs e)
        {

            if (File.Exists(inputFile.FileName))
                input_text = File.ReadAllText(inputFile.FileName);
            else
                if (textBox_Input.Text.Length > 0)
                    input_text = textBox_Input.Text;
                else
                    return;

            button_Convert.IsEnabled = false;
            clock = new Stopwatch();
            len = input_text.Length;

            clock.Start();
            progressBar1.Visibility = 0;
            progressBar1.Minimum = 1;
            progressBar1.Maximum = 100;
            progressBar1.Value = 1;
            int step = input_text.Length / 100 > 0 ? input_text.Length / 100 : 1;

            backgroundWorker.RunWorkerAsync();
        }

        private void button_Load_Click(object sender, RoutedEventArgs e)
        {
            inputFile.ShowDialog();
            if (inputFile.CheckFileExists)
            {
                textBox_Input.Text = File.ReadAllText(inputFile.FileName);
            }
        }

        private void MOpen_Click(object sender, RoutedEventArgs e)
        {
            inputFile.ShowDialog();
            if (inputFile.CheckFileExists)
            {
                textBox_Input.Text = File.ReadAllText(inputFile.FileName);
            }
        }

        private void MSave_Click(object sender, RoutedEventArgs e)
        {
            outputFile.ShowDialog();
        }

        private void MExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MI_Savetxt_Click(object sender, RoutedEventArgs e)
        {
            MI_Save_txt.IsChecked = true;
            MI_Save_pdf.IsChecked = false;
        }

        private void MI_Savepdf_Click(object sender, RoutedEventArgs e)
        {
            MI_Save_txt.IsChecked = false;
            MI_Save_pdf.IsChecked = true;
        }
        
        private void MI_Language_RU_Click(object sender, RoutedEventArgs e)
        {
            MI_Language_RU.IsChecked = true;
            MI_Language_EN.IsChecked = false;
        }

        private void MI_Language_EN_Click(object sender, RoutedEventArgs e)
        {
            MI_Language_RU.IsChecked = false;
            MI_Language_EN.IsChecked = true;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            var wAbout = new AboutWindow(this);
            wAbout.Show();
        }
               
        private void button_Save_Click(object sender, RoutedEventArgs e)
        {
            outputFile.ShowDialog();
        }
    }
}

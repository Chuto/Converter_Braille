using Converter_Braille.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Converter_Braille
{
    public class BGWorker
    {
        public BackgroundWorker worker;
        int len;
        string text;
        MainWindow mWindow;
        MemoryStream MS = new MemoryStream();
        private ManualResetEvent _workerCompleted = new ManualResetEvent(false);
        Stopwatch clock;

        public BGWorker(MainWindow parent, string text)
        {
            
            worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler
                    (ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler
                    (RunWorkerCompleted);
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;

            this.mWindow = parent;
            this.len = text.Length;
            this.text = text;
        }

        private void DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            int step = text.Length / 100 > 0 ? 1 : 100 / text.Length;

            int count_letter = 0;
            int count_line = 0;

            clock = new Stopwatch();
            clock.Start();
            
            for (int i = 0; i < len; i++)
            {

                if (MainWindow._dictionary.ContainsKey(text[i]))
                {
                    MS.Write(MainWindow._dictionary[text[i]].b, 0, 3);
                    count_letter++;
                    if (count_letter == Settings.letterCount)
                    {
                        MS.Write(new byte[] { 20, 20, 13 }, 0, 3);
                        count_line++;
                        count_letter = 0;
                    }
                    if (count_line == Settings.lineCount)
                    {
                        MS.Write(new byte[] { 20, 20, 13 }, 0, 3);
                        MS.Write(new byte[] { 20, 20, 13 }, 0, 3);
                        MS.Write(new byte[] { 20, 20, 13 }, 0, 3);
                        count_line = 0;
                    }
                }

                if (i % (int)((text.Length / 100) + 1) == 0 || step > 1)
                    worker.ReportProgress(step);
            }
            clock.Stop();
            _workerCompleted.Set();
        }
        private void ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            mWindow.progressBar1.Value += e.ProgressPercentage;
        }

        private void RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {            
            if (mWindow.outputFile.FileName != String.Empty && mWindow.outputFile.CheckPathExists)
                File.WriteAllText(mWindow.outputFile.FileName, Encoding.UTF8.GetString(MS.ToArray()), Encoding.UTF8);
            else
                mWindow.textBox_Output.Text = Encoding.UTF8.GetString(MS.ToArray());

            MS.Close();

            mWindow.progressBar1.Value = 1;
            
            mWindow.textBlock.Text = clock.Elapsed.ToString("mm\\:ss\\.fff");
            mWindow.button_Convert.IsEnabled = true;
        }
    }

}

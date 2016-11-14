using Converter_Braille.Models;
using Converter_Braille.Translater;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

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
            MemoryStream buffer = new MemoryStream();
            StringBuilder buf = new StringBuilder();

            var rul = new Rules(0, 0, true, true);
            int step = text.Length / 100 > 0 ? 1 : 100 / text.Length;            

            clock = new Stopwatch();
            clock.Start();

            for (int i = 0; i < len; i++)
            {
                if (MainWindow._dictionary.ContainsKey(text[i]))
                {
                    rul.count_letter++;
                    if (rul.count_line == Settings.GetInstance().lineCount)
                    {
                        MS.Write(new byte[] { 13, 10 }, 0, 2);
                        MS.Write(new byte[] { 13, 10 }, 0, 2);
                        MS.Write(new byte[] { 13, 10 }, 0, 2);
                        rul.count_line = 0;
                    }

                    if (text[i] == ' ')
                    {
                        if (rul.count_letter != Settings.GetInstance().letterCount)
                        {
                            buffer.Write(MainWindow._dictionary[text[i]].b, 0, 3);
                        }
                        MS.Write(buffer.GetBuffer(), 0, Convert.ToInt32(buffer.Length));
                        rul.bigchar = true;
                        buffer.Close();
                        buffer = new MemoryStream();
                    }
                    else
                    {
                        if ((('A' <= text[i] && text[i] <= 'Z') || ('А' <= text[i] && text[i] <= 'Я')) && rul.bigchar)
                        {
                            buffer.Write(new byte[] { 226, 160, 160 }, 0, 3); //правило заглавной буквы
                            rul.count_letter++;
                        }
                        if (('0' <= text[i] && text[i] <= '9'))
                        {
                            if (rul.digit)
                            {
                                buffer.Write(new byte[] { 226, 160, 188 }, 0, 3);
                                rul.digit = false;
                                rul.count_letter++;
                            }
                        }
                        else
                            rul.digit = true;

                        rul.bigchar = false;
                        buffer.Write(MainWindow._dictionary[text[i]].b, 0, 3);
                    }


                    if (rul.count_letter >= Settings.GetInstance().letterCount)
                    {
                        if (Encoding.UTF8.GetString(buffer.ToArray()).Length >= Settings.GetInstance().letterCount)
                        {                       
                            MS.Write(buffer.GetBuffer(), 0, Convert.ToInt32(buffer.Length));
                            buffer.Close();
                            buffer = new MemoryStream();
                            rul.count_letter = 0;
                        }
                        else
                        {
                            rul.count_letter = Encoding.UTF8.GetString(buffer.ToArray()).Length;
                        }
                        MS.Write(new byte[] { 13, 10 }, 0, 2);
                        rul.count_line++;
                    }

                }
                else
                    rul.bigchar = true;

                if (i % (int)((text.Length / 100) + 1) == 0 || step > 1)
                    worker.ReportProgress(step);
            }
            MS.Write(buffer.GetBuffer(), 0, Convert.ToInt32(buffer.Length));
            buffer.Close();
            clock.Stop();
            _workerCompleted.Set();
        }
        private void ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            mWindow.progressBar1.Value += e.ProgressPercentage;
        }

        private void RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            File.WriteAllText("result.txt", Encoding.UTF8.GetString(MS.ToArray()), Encoding.UTF8);
            if (Settings.GetInstance().preView)
                mWindow.textBox_Output.Text = Encoding.UTF8.GetString(MS.ToArray());
            MS.Close();
            mWindow.progressBar1.Value = 1;
            mWindow.textBlock.Text = clock.Elapsed.ToString("mm\\:ss\\.fff");
            mWindow.button_Convert.IsEnabled = true;
        }
    }
}

using Converter_Braille.Models;
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

            int step = text.Length / 100 > 0 ? 1 : 100 / text.Length;

            int count_letter = 0;
            int count_line = 0;

            clock = new Stopwatch();
            clock.Start();

            for (int i = 0; i < len; i++)
            {
                //if (MainWindow._dictionary.ContainsKey(text[i]))
                //{
                //    buffer.Write(MainWindow._dictionary[text[i]].b, 0, 3);
                //    count_letter++;

                //    if (text[i] == ' ')
                //    {
                //        MS.Write(buffer.GetBuffer(), 0, Convert.ToInt32(buffer.Length));
                //        buffer.Close();
                //        buffer = new MemoryStream();
                //    }

                //    if (count_letter == Settings.GetInstance().letterCount)
                //        if (i < text.Length && text[i+1] == ' ')
                //        {
                //            i++;
                //            MS.Write(buffer.GetBuffer(), 0, Convert.ToInt32(buffer.Length));
                //            MS.Write(new byte[] { 13, 10 }, 0, 2);

                //            buffer.Close();
                //            buffer = new MemoryStream();
                //            count_line++;
                //            count_letter = 0;
                //        }
                //        else
                //        {
                //            MS.Write(new byte[] { 13, 10 }, 0, 2);
                //            count_line++;
                //            count_letter = Convert.ToInt32(buffer.Length) / 3;
                //        }

                //    if (count_line == Settings.GetInstance().lineCount)
                //    {
                //        MS.Write(new byte[] { 13, 10 }, 0, 2);
                //        MS.Write(new byte[] { 13, 10 }, 0, 2);
                //        MS.Write(new byte[] { 13, 10 }, 0, 2);
                //        count_line = 0;
                //    }
                //}

                if (MainWindow._dictionary.ContainsKey(text[i]))
                {
                    if (count_line == 15)
                    {
                        MS.Write(new byte[] { 13, 10 }, 0, 2);
                        MS.Write(new byte[] { 13, 10 }, 0, 2);
                        MS.Write(new byte[] { 13, 10 }, 0, 2);
                        count_line = 0;
                    }

                    if (text[i] == ' ')
                    {
                        if (count_letter != 30)
                        {
                            buffer.Write(MainWindow._dictionary[text[i]].b, 0, 3);
                        }
                        MS.Write(buffer.GetBuffer(), 0, Convert.ToInt32(buffer.Length));
                        buffer.Close();
                        buffer = new MemoryStream();
                    }
                    else
                    {
                        buffer.Write(MainWindow._dictionary[text[i]].b, 0, 3);                        
                    }    
                               
                    if (count_letter == 30)
                    {
                        MS.Write(new byte[] { 13, 10 }, 0, 2);
                        count_line++;
                        count_letter = 0;
                    }


                    count_letter++;
                }

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
            if (mWindow.outputFile.FileName != String.Empty && mWindow.outputFile.CheckPathExists)
                File.WriteAllText(mWindow.outputFile.FileName, Encoding.UTF8.GetString(MS.ToArray()), Encoding.UTF8);
            else
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

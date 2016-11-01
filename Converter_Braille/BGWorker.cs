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
    struct Rules
    {
        bool digite;

    }

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
            bool bigchar = true, digit =true;


            clock = new Stopwatch();
            clock.Start();

            for (int i = 0; i < len; i++)
            {

                if (MainWindow._dictionary.ContainsKey(text[i]))
                {

                    count_letter++;
                    if (count_line == Settings.GetInstance().lineCount)
                    {
                        MS.Write(new byte[] {13, 10 }, 0, 2);
                        MS.Write(new byte[] {13, 10 }, 0, 2);
                        MS.Write(new byte[] {13, 10 }, 0, 2);
                        count_line = 0;
                    }


                    if (text[i] == ' ')
                    {
                        if (count_letter != Settings.GetInstance().letterCount)
                        {
                            buffer.Write(MainWindow._dictionary[text[i]].b, 0, 3);
                        }

                        MS.Write(buffer.GetBuffer(), 0, Convert.ToInt32(buffer.Length));
                        bigchar = true;
                        buffer.Close();
                        buffer = new MemoryStream();
                    }
                    else
                    {
                        if ((('A' <= text[i] && text[i] <= 'Z') || ('А' <= text[i] && text[i] <= 'Я')) && bigchar)
                        {
                            buffer.Write(new byte[] { 226, 160, 160 }, 0, 3);//правило заглавной буквы
                            count_letter++;
                        }
                        if (('0' <= text[i] && text[i] <= '9'))
                        {
                            if (digit)
                            { 
                                buffer.Write(new byte[] { 226, 160, 188 }, 0, 3);
                                digit = false;
                                count_letter++;
                            }
                        }
                        else
                            digit = true;

                        bigchar = false;
                        buffer.Write(MainWindow._dictionary[text[i]].b, 0, 3);                 
                    }

                       

                    if (count_letter >= Settings.GetInstance().letterCount)
                    {
                        if (Encoding.UTF8.GetString(buffer.ToArray()).Length >= Settings.GetInstance().letterCount)
                        {
                        //    MS.Write(new byte[] { 13, 10 }, 0, 2);                            
                            MS.Write(buffer.GetBuffer(), 0, Convert.ToInt32(buffer.Length));
                            buffer.Close();
                            buffer = new MemoryStream();
                            count_letter = 0;
                        }
                        else
                        {                            
                            count_letter = Encoding.UTF8.GetString(buffer.ToArray()).Length;
                        }
                        MS.Write(new byte[] { 13, 10 }, 0, 2);
                        count_line++;
                    }
                    
                }
                else
                    bigchar = true;

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

            pdf wq = new pdf();
            wq.createPdfFromImage("result.txt");

            mWindow.progressBar1.Value = 1;

            mWindow.textBlock.Text = clock.Elapsed.ToString("mm\\:ss\\.fff");
            mWindow.button_Convert.IsEnabled = true;
        }
    }

}

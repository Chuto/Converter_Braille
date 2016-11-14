using System;
using System.Windows;
using System.Windows.Controls;
using System.Drawing;
using System.Windows.Xps.Packaging;
using Microsoft.Office.Interop.Word;
using Word = Microsoft.Office.Interop.Word;
using System.IO;

namespace Converter_Braille
{
    public partial class UserGuideWindow : System.Windows.Window
    {
        MainWindow mainUG;

        public UserGuideWindow(MainWindow parent)
        {
            InitializeComponent();
            string wordDocument = @"H:\Study\Converter_Braille\Converter_Braille\Resources\UserGuide.docx";
            if (string.IsNullOrEmpty(wordDocument) || !File.Exists(wordDocument))
            {
                MessageBox.Show("Файл руководства пользователя не найден, пожалуйста, обратитесь к разработчику");
            }
            else
            {
                string convertedXpsDoc = string.Concat(Path.GetTempPath(), "\\", Guid.NewGuid().ToString(), ".xps");
                XpsDocument xpsDocument = ConvertWordToXps(wordDocument, convertedXpsDoc);
                if (xpsDocument == null)
                {
                    return;
                }
                documentViewer.Document = xpsDocument.GetFixedDocumentSequence();
            }
            mainUG = parent;
        }

        private XpsDocument ConvertWordToXps(string wordFilename, string xpsFilename)
        {
            Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            try
            {
                wordApp.Documents.Open(wordFilename);
                wordApp.Application.Visible = false;
                wordApp.WindowState = WdWindowState.wdWindowStateMinimize;
                Document doc = wordApp.ActiveDocument;
                doc.SaveAs(xpsFilename, WdSaveFormat.wdFormatXPS);
                XpsDocument xpsDocument = new XpsDocument(xpsFilename, FileAccess.Read);
                return xpsDocument;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurs, The error message is  " + ex.ToString());
                return null;
            }
            finally
            {
                wordApp.Documents.Close();
                ((_Application)wordApp).Quit(WdSaveOptions.wdDoNotSaveChanges);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mainUG.IsEnabled = true;
        }
    }
}
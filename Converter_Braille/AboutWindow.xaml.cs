using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Converter_Braille
{
    public partial class AboutWindow : Window
    {
        MainWindow mainWin;

        public AboutWindow(MainWindow parent)
        {
            mainWin = parent;
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mainWin.IsEnabled = true;
        }
    }
}

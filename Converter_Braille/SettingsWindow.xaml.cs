using Converter_Braille.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Converter_Braille
{
       public partial class SettingsWindow : Window
    {
        MainWindow main;

        public SettingsWindow(MainWindow parent)
        {
            main = parent;
            InitializeComponent();
            this.textBox_letter.Text = Settings.GetInstance().letterCount.ToString();
            this.textBox_line.Text = Settings.GetInstance().lineCount.ToString();
            this.checkBox_preView.IsChecked = Settings.GetInstance().preView;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            main.IsEnabled = true;
        }

        private void button_save_Click(object sender, RoutedEventArgs e)
        {
            Settings.GetInstance().letterCount = Int32.Parse(this.textBox_letter.Text);
            Settings.GetInstance().lineCount = Int32.Parse(this.textBox_line.Text);
            Settings.GetInstance().preView = this.checkBox_preView.IsChecked ?? true;
            main.config.Save();
            this.Close();
        }
    }
}

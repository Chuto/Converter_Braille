using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Converter_Braille.Models
{
    public class Settings
    {
        private static Settings Instance;

        public int letterCount { get; set; }
        public int lineCount { get; set; }
        public bool preView { get; set; }

        public static Settings GetInstance()
        {
            if (Instance == null)
                Instance = new Settings();
            return Instance;
        }
    }
}

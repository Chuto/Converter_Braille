using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Converter_Braille.Models
{
    public class Letter
    {
        public char s { get; set; }
        public byte[] a { get; set; }
        public byte[] u { get; set; }
        public byte[] b { get; set; }
    }
}

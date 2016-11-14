using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Converter_Braille.Models
{
    public class Rules
    {
        public int count_letter { get; set; }
        public int count_line { get; set; }
        public bool bigchar { get; set; }
        public bool digit { get; set; }

        public Rules(int cle, int cli, bool bc, bool d)
        {
            count_letter = cle;
            count_line = cli;
            bigchar = bc;
            digit = d;
        }
    }
}

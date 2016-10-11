using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Converter_Braille.Models;

namespace Converter_Braille.Translater
{
    public class Converter
    {
        MemoryStream result { get; set; }

        public MemoryStream Convert(String inputText, Dictionary<char, Letter> dictionary)
        {
            result = new MemoryStream();
            for (int i = 0; i < inputText.Length; i++)
            {
                if (dictionary.ContainsKey(inputText[i]))
                    result.Write(dictionary[inputText[i]].b, 0, 3);
            } 
            return result;
        }

        public static unsafe byte[] Copy(byte[] source, int sourceOffset, int count)
        {
            byte[] target = new byte[count];
            int targetOffset = 0;
            fixed (byte* pSource = source, pTarget = target)
            {
                byte* ps = pSource + sourceOffset;
                byte* pt = pTarget + targetOffset;

                for (int i = 0; i < count; i++)
                {
                    *pt = *ps;
                    pt++;
                    ps++;
                }
                return target;
            }
        }
    }
}

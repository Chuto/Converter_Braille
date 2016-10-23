using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Converter_Braille.Translater
{
    public class pdf
    {
        public void createPdfFromImage(string FileName)
        {
            var output = "Output.pdf";

            string ttf = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "seguisym.ttf");
            var baseFont = BaseFont.CreateFont(ttf, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            var font = new Font(baseFont, 22.0f , iTextSharp.text.Font.NORMAL);

            using (FileStream fs = new FileStream(output, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (Document doc = new Document(PageSize.A4, 30, 30, 15, 15))
                {
                    using (PdfWriter writer = PdfWriter.GetInstance(doc, fs))
                    {
                        //Open document for writing
                        doc.Open();
                        doc.Add(new Paragraph(File.ReadAllText(FileName),font));
                        doc.Close();
                    }
                }
            }
        }

    }
}

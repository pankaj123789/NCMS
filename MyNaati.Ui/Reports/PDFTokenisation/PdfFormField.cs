//using System.Linq;
//using PdfSharp.Drawing;
//using PdfSharp.Drawing.Layout;
//using PdfSharp.Pdf;
//using PdfSharp.Pdf.AcroForms;
//using PdfSharp.Pdf.Annotations;

//namespace MyNaati.Ui.Reports.PDFTokenisation
//{
//    public class PdfFormField
//    {
//        public bool Handled { get; set; }

//        public XRect Rectangle { get; set; }
//        public string Title { get; set; }
//        public XFont Font { get; set; }
//        public XStringAlignment Alignment { get; set; }
//        public XBrush Brush { get; set; }

//        private PdfAcroField mField;
//        public PdfAcroField Field
//        {
//            get { return mField; }
//            set
//            {
//                mField = value;
//                if (mField != null)
//                    SetFontAndAlignment(mField);
//            }
//        }

//        public PdfFormField(string title, PdfAnnotation annotation, PdfPage page)
//        {
//            Alignment = XStringAlignment.Near;

//            Title = title;
//            //the page goes from the bottom
//            Rectangle = new XRect(annotation.Rectangle.X1, page.Height - annotation.Rectangle.Y1 - annotation.Rectangle.Height, annotation.Rectangle.Width, annotation.Rectangle.Height);
//            SetFontAndAlignment(annotation);
//        }

//        private void SetFontAndAlignment(PdfDictionary item)
//        {
//            var fontCode = item.Elements[PdfAcroField.Keys.DA] as PdfString;
//            if (fontCode != null)
//            {
//                Font = FontFromPdfFont(fontCode);
//                Brush = BrushFromPdfFont(fontCode);
//            }

//            //grab the alignment
//            var alignmentCode = item.Elements[PdfAcroField.Keys.Q] as PdfInteger;
//            if (alignmentCode != null)
//            {
//                switch (alignmentCode.Value)
//                {
//                    case 0:
//                        Alignment = XStringAlignment.Near;
//                        break;
//                    case 1:
//                        Alignment = XStringAlignment.Center;
//                        break;
//                    case 2:
//                        Alignment = XStringAlignment.Far;
//                        break;
//                }
//            }
//        }

//        public void RenderSimpleTextItem(XGraphics gfx, PdfFormField field, string output)
//        {
//            var font = field.Font;
//            var brush = field.Brush;

//            var format = XStringFormats.TopLeft;
//            if (field.Field.Flags != PdfAcroFieldFlags.Multiline)
//            {
//                format.LineAlignment = XLineAlignment.Center;
//                format.Alignment = field.Alignment;
//            }

//            output = output ?? string.Empty;
//            output = output.Replace("\r\n", "\n");

//            if (field.Field.Flags == PdfAcroFieldFlags.Multiline)
//            {
//                var tf = new XTextFormatter(gfx);
//                tf.DrawString(output, font, brush, field.Rectangle, XStringFormats.TopLeft);
//            }
//            else
//                gfx.DrawString(output, font, brush, field.Rectangle, format);
//        }

//        private XFont FontFromPdfFont(PdfString font)
//        {
//            const string boldItalic = "Bold,Italic";
//            const string bold = "Bold";
//            const string italic = "Italic";
//            const string fontHelv = "Helvetica";
//            const string fontTiRo = "Times";
//            const string fontCour = "Courier New";
//            const string fontSymb = "Symbol";
//            const string fontArial = "Arial";

//            var options = new XPdfFontOptions(PdfFontEncoding.WinAnsi, PdfFontEmbedding.Default);

//            if (font == null) return null;

//            var pieces = font.Value.Split(' ');

//            var family = pieces[0].Replace("/", string.Empty);
//            double size;
//            if (!double.TryParse(pieces[1], out size))
//                return new XFont("Arial", 10);

//            var split = family.Split(',');
//            var searchfam = split[0];
//            var modifier = string.Join(",", split.Reverse().Take(split.Count() - 1).Reverse());
//            switch (family)
//            {
//                case "Helv": searchfam = fontHelv; break;
//                case "HeBO": searchfam = fontHelv; modifier = boldItalic; break;
//                case "HeBo": searchfam = fontHelv; modifier = bold; break;
//                case "HeOb": searchfam = fontHelv; modifier = italic; break;
//                case "TiRo": searchfam = fontTiRo; break;
//                case "TiBI": searchfam = fontTiRo; modifier = boldItalic; break;
//                case "TiIt": searchfam = fontTiRo; modifier = italic; break;
//                case "TiBo": searchfam = fontTiRo; modifier = bold; break;
//                case "Cour": searchfam = fontCour; break;
//                case "CoBo": searchfam = fontCour; modifier = bold; break;
//                case "CoOb": searchfam = fontCour; modifier = italic; break;
//                case "CoBO": searchfam = fontCour; modifier = boldItalic; break;
//                case "Symb": searchfam = fontSymb; break;
//                case "CourierNew": searchfam = fontCour; break;
//                case "Arial": searchfam = fontArial; break;
//                case "Arial,Bold": searchfam = fontArial; modifier = bold; break;
//            }
//            if (!string.IsNullOrEmpty(modifier))
//                searchfam = string.Join(",", new[] { searchfam, modifier });

//            try
//            {
//                if (searchfam == "Arial,Bold")
//                    return new XFont("Arial", size, XFontStyle.Bold, options);

//                return new XFont(searchfam, size, XFontStyle.Regular, options);
//            }
//            catch
//            {
//                return new XFont(fontHelv, size, XFontStyle.Regular, options);
//            }
//        }

//        private XBrush BrushFromPdfFont(PdfString font)
//        {
//            if (font != null)
//            {
//                var pieces = font.Value.Split(' ');
//                switch (pieces.Last())
//                {
//                    case "g": // grayscale
//                        double gray;
//                        if (double.TryParse(pieces[pieces.Count() - 2], out gray))
//                            return new XSolidBrush(XColor.FromGrayScale(gray));
//                        break;
//                    case "rg": // rgb
//                        double red, green, blue;
//                        var success = new bool[3];
//                        if (pieces.Count() < 4) break;
//                        success[0] = double.TryParse(pieces[pieces.Count() - 4], out red);
//                        success[1] = double.TryParse(pieces[pieces.Count() - 3], out green);
//                        success[2] = double.TryParse(pieces[pieces.Count() - 2], out blue);
//                        if (success.All(v => v))
//                            return new XSolidBrush(XColor.FromArgb((int)(255 * red), (int)(255 * green), (int)(255 * blue)));
//                        break;
//                }
//            }

//            return XBrushes.Black;
//        }
//    }
//}
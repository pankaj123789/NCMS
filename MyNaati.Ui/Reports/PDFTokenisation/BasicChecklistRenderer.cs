//using System;
//using System.Collections.Generic;
//using System.Linq;
//using iTextSharp.text.pdf;
//using PdfSharp.Drawing;

//namespace MyNaati.Ui.Reports.PDFTokenisation
//{
//    public abstract class BasicChecklistRenderer : ITokenRenderer
//    {
//        protected abstract List<string> ItemList { get; }

//        protected abstract string FieldTitle { get; }

//        protected virtual double BoxSize
//        {
//            get { return 10; }
//        }

//        protected virtual double TextLeft
//        {
//            get { return 15; }
//        }

//        protected virtual double BoxMarginLeft
//        {
//            get { return 0; }
//        }

//        protected virtual double BoxMarginTop
//        {
//            get { return 2; }
//        }

//        public void Render(XGraphics gfx, List<PdfFormField> fields)
//        {
//            //only if we have the field do we do anything
//            var field = fields.SingleOrDefault(e => string.Equals(e.Title, FieldTitle));
//            if (field == null)
//                return;

//            double currentY = field.Rectangle.Top;

//            //render each item with a box to the left of it
//            foreach (var item in ItemList)
//            {
//                var textRectangle = new XRect(field.Rectangle.Left + TextLeft, currentY, field.Rectangle.Width - TextLeft, field.Rectangle.Height); //don't care about the height remaining in the field atm

//                var wrapper = new PdfWordWrapper(gfx, textRectangle.Width);

//                var writeString = (item ?? string.Empty);
//                writeString = writeString.Replace("\r\n", "\n");

//                //split up into regular and bold portions
//                string[] writeStringPieces = writeString.Split(new string[] { "[b]", "[/b]" }, StringSplitOptions.None);

//                for (int i = 0; i < writeStringPieces.Length; i++)
//                {
//                    if (i % 2 == 0)
//                    {
//                        wrapper.Add(writeStringPieces[i], field.Font, field.Brush);
//                    }
//                    else
//                    {
//                        var boldFont = new XFont(field.Font.FontFamily.Name, field.Font.Size, XFontStyle.Bold);
//                        wrapper.Add(writeStringPieces[i], boldFont, field.Brush);
//                    }
//                }

//                wrapper.Process();

//                //this can be used for debugging to see what we've got
//                //gfx.DrawRectangle(XPens.Red, textRectangle.Left, textRectangle.Top, wrapper.Size.Width, wrapper.Size.Height);

//                //draw a box at the right place
//                gfx.DrawRectangle(new XPen(XColors.Black, 0.5), field.Rectangle.Left + BoxMarginLeft, currentY + BoxMarginTop, BoxSize, BoxSize);

//                //convert field alignment to wrapper alignment
//                PdfWordWrapper.Alignment wrapperAlignment = PdfWordWrapper.Alignment.Left;
//                if (field.Alignment == XStringAlignment.Far)
//                {
//                    wrapperAlignment = PdfWordWrapper.Alignment.Right;
//                }
//                else if (field.Alignment == XStringAlignment.Center)
//                {
//                    wrapperAlignment = PdfWordWrapper.Alignment.Center;
//                }

//                wrapper.Draw(gfx, textRectangle.Left, textRectangle.Top, wrapperAlignment);

//                currentY += wrapper.Size.Height;
//            }
//        }

//        public void Render(IList<AcroFields.FieldPosition> positions, PdfStamper stamper, PdfReader reader)
//        {
//        }

//        public int FieldCount()
//        {
//            // a list will always be one field
//            return 1;
//        }

//        public string[] FieldTitles()
//        {
//            return new string[] { FieldTitle };
//        }
//    }
//}
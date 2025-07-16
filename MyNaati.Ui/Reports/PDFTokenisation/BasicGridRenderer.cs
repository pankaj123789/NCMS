//using System;
//using System.Collections.Generic;
//using System.Linq;
//using iTextSharp.text.pdf;
//using PdfSharp.Drawing;

//namespace MyNaati.Ui.Reports.PDFTokenisation
//{
//    public abstract class BasicGridRenderer : ITokenRenderer
//    {
//        private const double EPSILON = 0.02;

//        protected abstract List<string[]> ItemList { get; }

//        protected abstract string[] FieldTitles { get; }

//        protected virtual double MarginLeft { get { return 2; } }
//        protected virtual double MarginRight { get { return 2; } }
//        protected virtual double SpacingTop { get { return 2; } }
//        protected virtual double SpacingBottom { get { return 2; } }

//        public void Render(XGraphics gfx, List<PdfFormField> fields)
//        {
//            //join the fields in with the item list
//            var adjustedRowList = GetArrayObj(fields);

//            if (adjustedRowList.Count() == 0 || adjustedRowList.First().Where(e => e.Field != null).Count() == 0)
//                return;

//            var currentY = adjustedRowList.First().First(e => e.Field != null).Field.Rectangle.Top + SpacingTop;
//            var leftX = adjustedRowList.First().Where(e => e.Field != null).Select(e => e.Field.Rectangle.Left).Min();
//            var rightX = adjustedRowList.First().Where(e => e.Field != null).Select(e => e.Field.Rectangle.Right).Max();

//            foreach (var adjustedRow in adjustedRowList)
//            {
//                double thisHeight = 0;

//                foreach (var item in adjustedRow.Where(e => e.Field != null))
//                {
//                    XRect textRectangle = new XRect(
//                        item.Field.Rectangle.Left + ((Math.Abs(item.Field.Rectangle.Left - leftX) < EPSILON) ? MarginLeft : 0),
//                        currentY,
//                        item.Field.Rectangle.Width - ((Math.Abs(item.Field.Rectangle.Left - leftX) < EPSILON) ? MarginLeft : 0) - ((Math.Abs(item.Field.Rectangle.Right - rightX) < EPSILON) ? MarginRight : 0),
//                        item.Field.Rectangle.Height);


//                    PdfWordWrapper wrapper = new PdfWordWrapper(gfx, textRectangle.Width);

//                    var writeString = item.Text ?? string.Empty;
//                    writeString = writeString.Replace("\r\n", "\n");

//                    wrapper.Add(writeString, item.Field.Font, item.Field.Brush);
//                    wrapper.Process();

//                    //this can be used for debugging to see what we've got
//                    //gfx.DrawRectangle(XPens.Red, textRectangle.Left, textRectangle.Top, wrapper.Size.Width, wrapper.Size.Height);

//                    //convert field alignment to wrapper alignment
//                    PdfWordWrapper.Alignment wrapperAlignment = PdfWordWrapper.Alignment.Left;
//                    if (item.Field.Alignment == XStringAlignment.Far)
//                        wrapperAlignment = PdfWordWrapper.Alignment.Right;
//                    else if (item.Field.Alignment == XStringAlignment.Center)
//                        wrapperAlignment = PdfWordWrapper.Alignment.Center;

//                    wrapper.Draw(gfx, textRectangle.Left, textRectangle.Top, wrapperAlignment);

//                    //get maximum height of strings rendered
//                    if (thisHeight < wrapper.Size.Height)
//                        thisHeight = wrapper.Size.Height;
//                }

//                currentY += thisHeight + SpacingBottom;
//                //draw a line across in the rectangle
//                gfx.DrawLine(new XPen(XColors.Black, 0.5), leftX, currentY, rightX, currentY);
//                currentY += SpacingTop;
//            }
//        }

//        public int FieldCount()
//        {
//            return FieldTitles.Count();
//        }

//        string[] ITokenRenderer.FieldTitles()
//        {
//            return FieldTitles;
//        }

//        public void Render(IList<AcroFields.FieldPosition> positions, PdfStamper stamper, PdfReader reader)
//        {
//            PdfPTable table = new PdfPTable(positions.Count);

//            table.TotalWidth = FullWidth(positions);

//            table.LockedWidth = true;

//            var relativeWidths = GetRelativeWidths(positions);

//            table.SetWidths(relativeWidths);

//            var page = stamper.GetOverContent(positions[0].page);

//            foreach (var row in ItemList)
//            {
//                foreach (var cell in row)
//                {
//                    table.AddCell(cell);
//                }
//            }

//            table.WriteSelectedRows(0, ItemList.Count, positions[0].position.Left, positions[0].position.Top,
//                page);

//        }

//        private static float[] GetRelativeWidths(IList<AcroFields.FieldPosition> positions)
//        {
//            float[] relativeWidths = new float[positions.Count];
//            int i = 0;
//            foreach (var pos in positions)
//            {
//                relativeWidths[i] = pos.position.Width;
//                i++;
//            }
//            return relativeWidths;
//        }

//        private static float FullWidth(IList<AcroFields.FieldPosition> positions)
//        {
//            float fullWidth = 0.0f;
//            foreach (var pos in positions)
//            {
//                fullWidth += pos.position.Width;
//            }
//            return fullWidth;
//        }

//        public class FormFieldRenderObject
//        {
//            private readonly string mText;
//            private readonly PdfFormField mField;

//            public string Text
//            {
//                get { return mText; }
//            }

//            public PdfFormField Field
//            {
//                get { return mField; }
//            }

//            public FormFieldRenderObject(string text, PdfFormField field)
//            {
//                mText = text;
//                mField = field;
//            }
//        }

//        private List<FormFieldRenderObject[]> GetArrayObj(List<PdfFormField> fields)
//        {
//            List<FormFieldRenderObject[]> objectList = new List<FormFieldRenderObject[]>();
//            for (int i = 0; i < ItemList.Count; i++)
//            {
//                List<FormFieldRenderObject> objectItemList = new List<FormFieldRenderObject>();
//                for (int j = 0; j < ItemList[i].Count(); j++)
//                    objectItemList.Add(new FormFieldRenderObject(ItemList[i][j], fields.SingleOrDefault(e => string.Equals(e.Title, FieldTitles[j]))));
//                objectList.Add(objectItemList.ToArray());
//            }
//            return objectList;
//        }
//    }
//}
//using System.Collections.Generic;
//using System.IO;
//using iTextSharp.text.pdf;

//namespace MyNaati.Ui.Reports.PDFTokenisation
//{
//    public class PdfRenderer : IPdfRenderer
//    {
//        private List<ITokenReplacement> mTokenReplacements = new List<ITokenReplacement>();
//        private List<ITokenRenderer> mTokenRenderers = new List<ITokenRenderer>();

//        public void RegisterTokenReplacements(IEnumerable<ITokenReplacement> replacements)
//        {
//            mTokenReplacements.AddRange(replacements);
//        }

//        public void RegisterComplexTokenRenderers(IEnumerable<ITokenRenderer> renderers)
//        {
//            mTokenRenderers.AddRange(renderers);
//        }

//        public byte[] RenderDocument(byte[] fileBytes)
//        {
//            var memoryStream = new MemoryStream(fileBytes, false);
//            var outputStream = new MemoryStream();

//            //PdfDocument document = PdfReader.Open(memoryStream, PdfDocumentOpenMode.Import);
//            var document = new PdfReader(memoryStream);
//            PdfStamper pdfStamper = new PdfStamper(document, outputStream);
//            AcroFields pdfFormFields = pdfStamper.AcroFields;

//            //var outputDoc = new PdfDocument();
//            //outputDoc.Version = 14;

//            IList<AcroFields.FieldPosition> positions = new List<AcroFields.FieldPosition>();

//            int i = 0;

//            foreach (var de in document.AcroFields.Fields)
//            {
//                //sb.Append(de.Key.ToString() + Environment.NewLine);
//                string replacementText = null;
//                foreach (var tokenReplacement in mTokenReplacements)
//                {
//                    replacementText = tokenReplacement.GetReplacement(de.Key);
//                    if (replacementText != null)
//                    {
//                        break;
//                    }
//                }
//                if (replacementText != null)
//                {
//                    pdfFormFields.SetField(de.Key, replacementText);
//                }
//                else
//                {
//                    bool foundComplexRenderer = false;
//                    foreach (var fieldTitles in mTokenRenderers)
//                    {
//                        foreach (var fieldTitle in fieldTitles.FieldTitles())
//                        {
//                            if (de.Key == fieldTitle)
//                            {
//                                foundComplexRenderer = true;
//                            }
//                        }
//                    }
//                    if (foundComplexRenderer)
//                    {
//                        foreach (AcroFields.FieldPosition pos in pdfFormFields.GetFieldPositions(de.Key))
//                        {
//                            positions.Add(pos);
//                            pdfStamper.AcroFields.SetFieldProperty(de.Key, "setfflags",
//                                iTextSharp.text.pdf.PdfFormField.FF_READ_ONLY, null);
//                        }
//                        if (positions.Count == mTokenRenderers[i].FieldCount())
//                        {
//                            mTokenRenderers[i].Render(positions, pdfStamper, document);
//                            positions.Clear();
//                            i++;
//                        }
//                    }
//                }
//            }

//            pdfStamper.FormFlattening = false;

//            pdfStamper.Close();

//            return outputStream.ToArray();
//        }
//    }
//}
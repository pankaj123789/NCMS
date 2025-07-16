using System;
using System.Drawing;
using System.IO;
using System.Linq;
using GemBox.Spreadsheet;
using Ncms.Contracts.Models.Common;

namespace Ncms.Bl.Export
{
    public abstract class SearchResultsExporter
    {
        protected SearchResultsExporter()
        {
            _defaultDataCellStyle = new CellStyle();
            _defaultDataCellStyle.Borders.SetBorders(MultipleBorders.Outside, Color.Black, LineStyle.Thin);
            _defaultDataCellStyle.WrapText = true;
        }

        static SearchResultsExporter()
        {
            SpreadsheetInfo.SetLicense("E43X-CVYJ-CTFZ-DO1S");
        }

        private readonly CellStyle _defaultDataCellStyle;
        protected virtual CellStyle DataCellStyle => _defaultDataCellStyle;
        protected virtual string[] Criteria => null;
        protected abstract object[][][] Data { get; }
        protected abstract string TemplateFileName { get; }

        protected const string TemplateRelativePath = "Resources/";
        protected const string FirstDataRowRangeName = "FirstDataRow";
        protected const string CriteriaRangeName = "SearchCriteria";
        protected const string HeaderFormatCellRangeName = "HeaderFormatCell";

        protected string GetTemplatePath(string templateFileName)
        {
            return $"{AppDomain.CurrentDomain.BaseDirectory}{TemplateRelativePath}{templateFileName}";
        }

        protected bool NamedRangeExists(ExcelWorksheet workSheet, string rangeName)
        {
            return workSheet.NamedRanges[rangeName] != null;
        }

        private void ValidateNamedRange(ExcelFile excelDoc, int worksheetIndex, string rangeName)
        {
            ExcelWorksheet worksheet = excelDoc.Worksheets[worksheetIndex];
            if (worksheet.NamedRanges[rangeName] == null)
            {
                throw new Exception($"The Excel template file is invalid. Worksheet {worksheetIndex + 1} does not contain the following named range: {rangeName}.");
            }
        }

        protected void ValidateTemplate(ExcelFile excelDoc)
        {
            if (Data == null)
            {
                throw new Exception("Can't validate Excel template when Data property is null.");
            }

            if (excelDoc.Worksheets.Count < Data.Length)
            {
                throw new Exception($"The Excel template has fewer worksheets ({excelDoc.Worksheets.Count}) than required to accommodate the supplied datasets ({Data.Length}).");
            }

            for (var worksheetIndex = 0; worksheetIndex < excelDoc.Worksheets.Count; worksheetIndex++)
            {
                ValidateNamedRange(excelDoc, worksheetIndex, FirstDataRowRangeName);
                ValidateNamedRange(excelDoc, worksheetIndex, HeaderFormatCellRangeName);
                ValidateNamedRange(excelDoc, worksheetIndex, CriteriaRangeName);
            }
        }

        protected void WriteRowValues(ExcelWorksheet workSheet, int row, int columnOffset, object[] values)
        {
            for (int column = columnOffset; column < values.Length + columnOffset; column++)
                workSheet.Cells[row, column].Value = values[column - columnOffset];
        }

        protected ExcelFile CreateDocument(string templateFileName)
        {
            string templatePath = GetTemplatePath(templateFileName);

            if (!File.Exists(templatePath))
            {
                throw new Exception("The Excel template file is missing.");
            }

            ExcelFile excelDoc = new ExcelFile();
            excelDoc.LoadXlsx(templatePath, XlsxOptions.PreserveMakeCopy);

            // i would just use int.MaxValue here but that causes gembox to break
            excelDoc.GroupMethodsAffectedCellsLimit = 100000;

            return excelDoc;
        }

        protected ExcelFile ExportToExcel(ExcelFile doc = null, int rowOffset = 0, int columnOffset = 0)
        {
            if (Data == null)
            {
                throw new Exception("Data has not been provided. Nothing to export to Excel.");
            }

            ExcelFile excelDoc = doc ?? CreateDocument(TemplateFileName);

            ValidateTemplate(excelDoc);

            for (var worksheetIndex = 0; worksheetIndex < Data.Length; worksheetIndex++)
            {
                ExportToWorksheet(excelDoc, worksheetIndex, Data[worksheetIndex], rowOffset, columnOffset);
            }

            excelDoc.Worksheets.ActiveWorksheet = excelDoc.Worksheets[0];

            return excelDoc;
        }

        protected void ExportToWorksheet(ExcelFile excelDoc, int workSheetIndex, object[][] data, int rowOffset = 0,
            int columnOffset = 0)
        {
            if (excelDoc.Worksheets.Count < workSheetIndex + 1)
            {
                throw new Exception($"The Excel template has fewer worksheets ({excelDoc.Worksheets.Count}) than required (at least {workSheetIndex + 1}).");
            }

            ExcelWorksheet worksheet = excelDoc.Worksheets[workSheetIndex];
            CellRange criteriaRange = worksheet.NamedRanges[CriteriaRangeName].Range;

            var searchCriteria = Criteria.ToArray();
            // write search details
            if (Criteria != null && Criteria.Any())
            {
                int maxCriteria = Math.Min(Criteria.Length, criteriaRange.Height);
                for (int i = 0; i < maxCriteria; i++)
                    criteriaRange[i, 0].Value = searchCriteria[i];
            }
            else
            {
                // if no criteria to show, clear the formatting in the range so it doesn't look odd
                criteriaRange.Style = new CellStyle(excelDoc);
            }

            // write data
            CellRange firstRowRange = worksheet.NamedRanges[FirstDataRowRangeName].Range;
            int column = firstRowRange.FirstColumnIndex + columnOffset;
            int row = firstRowRange.FirstRowIndex + rowOffset;
            foreach (object[] dataRow in data)
            {
                WriteRowValues(worksheet, row++, column, dataRow);
            }

            FormatWorkSheet(worksheet, firstRowRange, data.Length, worksheet.NamedRanges[HeaderFormatCellRangeName].Range[0].Style, DataCellStyle);
        }

        protected Stream ExcelToStream(ExcelFile excelDoc, FileType formatType, Stream stream = null)
        {

            if (stream == null)
            {
                stream = new MemoryStream();
            }

            var streamWriter = new StreamWriter(stream, System.Text.Encoding.UTF8);
            if (formatType.Equals(FileType.Xlsx))
            {
                excelDoc.SaveXlsx(stream);
            }
            else if (formatType.Equals(FileType.Csv))
            {
                excelDoc.SaveCsv(streamWriter, CsvType.CommaDelimited);
            }
            else
            {
                throw new InvalidOperationException("Unsupported export format type.");
            }
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        protected void FormatRange(ExcelWorksheet worksheet, int startRow, int startCol, int endRow, int endCol, CellStyle style)
        {
            CellRange range = worksheet.Cells.GetSubrangeAbsolute(startRow, startCol, endRow, endCol);
            range.Style = style;
        }

        protected virtual void FormatWorkSheet(ExcelWorksheet worksheet, CellRange firstRowRange, int rows, CellStyle headerCellStyle, CellStyle dataCellstyle)
        {
            if (rows <= 0)
                return;

            CellRange dataRange = worksheet.Cells.GetSubrangeAbsolute(firstRowRange.FirstRowIndex, firstRowRange.FirstColumnIndex,
                firstRowRange.FirstRowIndex + rows - 1, firstRowRange.LastColumnIndex);

            dataRange.Style = dataCellstyle;

            foreach (ExcelCell cell in dataRange)
            {
                if (cell.Value != null && cell.Value.ToString().IndexOf("=HYPERLINK", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    cell.Formula = cell.Value.ToString();
                    cell.Style.Font.UnderlineStyle = UnderlineStyle.Single;
                    cell.Style.Font.Color = Color.FromArgb(0, 0, 255);
                }
            }
        }

        public virtual Stream Export(FileType formatType, Stream stream = null)
        {
            ExcelFile excelDoc = ExportToExcel();
            return ExcelToStream(excelDoc, formatType, stream);
        }
    }
}
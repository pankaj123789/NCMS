using System;

namespace Ncms.Contracts.Models.Common
{
    public class FileType : ExtendedValueType<FileType>
    {
        private const string ApplicationOctetStream = "application/octet-stream";
        private const string ImagePng = "image/png";
        private const string ApplicationExcel = "application/vnd.ms-excel";
        private const string TextCsv = "text/csv";

        private const string PdfExtension = "pdf";
        private const string ZipExtension = "zip";
        private const string PngExtension = "png";
        private const string XlsxExtension = "xlsx";
        private const string CsvExtension = "csv";

        public string Extension => _data;
        public string MediaType { get; private set; }

        private FileType(string extension, string mediaType)
            : base(extension)
        {
            MediaType = mediaType;
        }

        private FileType() { }

        protected override void LoadType()
        {
            AddDefinition(new FileType(String.Empty, ApplicationOctetStream));
            AddDefinition(new FileType(PdfExtension, ApplicationOctetStream));
            AddDefinition(new FileType(ZipExtension, ApplicationOctetStream));
            AddDefinition(new FileType(PngExtension, ImagePng));
            AddDefinition(new FileType(XlsxExtension, ApplicationExcel));
            AddDefinition(new FileType(CsvExtension, TextCsv));
        }

        public static FileType Generic => Parse(String.Empty);
        public static FileType Pdf => Parse(PdfExtension);
        public static FileType Zip => Parse(ZipExtension);
        public static FileType Png => Parse(PngExtension);
        public static FileType Xlsx => Parse(XlsxExtension);
        public static FileType Csv => Parse(CsvExtension);

    }
}

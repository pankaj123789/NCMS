using System;
using F1Solutions.Naati.Common.Contracts.Bl.Attributes;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Contracts.Models.MaterialRequest.Wizard
{
    public class DocumentsUploadStepModel
    {
        public string WizardInstanceId { get; set; }

        
        public DocumentUploadModel[] Documents { get; set; }
    }


    public class DocumentUploadModel
    {
        public int TempFileId { get; set; }

        [LookupDisplay(LookupType.DocumentType, "DocumentTypeDisplayName")]
        public int DocumenTypeId { get; set; }

        public string Title { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedDateTime { get; set; }
        public string FilePath { get; set; }

        public bool ExaminersAvailable { get; set; }
        public bool MergeDocument { get; set; }
    }
}

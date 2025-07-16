using System;
using System.Collections.Generic;

namespace Ncms.Contracts.Models.Application
{
    public class CredentialApplicationFieldModel
    {
        public int Id { get; set; }
        public int FieldDataId { get; set; }
        public int FieldTypeId { get; set; }
        public string Name { get; set; }
        public int DataTypeId { get; set; }
        public string DefaultValue { get; set; }
        public string Value { get; set; }
        public bool PerCredentialRequest { get; set; }
        public string Description { get; set; }
        public bool Mandatory { get; set; }
        public bool Disabled { get; set; }
        public bool DisplayNone { get; set; }
        public int FieldOptionId { get; set; }
        public int DisplayOrder { get; set; }
        public bool FieldEnable { get; set; }

        public IEnumerable<CredentialApplicationFieldOptionModel> Options { get; set; }
    }

    public class WorkPracticeDataModel
    {
        public int WorkPracticeCredentialRequestId { get; set; }
        public int WorkPracticeId { get; set; }

        public int ObjectStatusId { get; set; }
    }

    public class CandidateBriefModel
    {
        public  int CandidateBriefId { get; set; }
        public  int TestMaterialAttachmentId { get; set; }
        public  int TestSittingId { get; set; }
        public  DateTime? EmailedDate { get; set; }
    }
}

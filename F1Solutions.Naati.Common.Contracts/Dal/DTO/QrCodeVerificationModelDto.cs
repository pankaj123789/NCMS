using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class QrCodeVerificationModelDto
    {
        public QrCodeVerificationModelDto()
        {
            QrCodeModelDtos = new List<QrCodeModelDto>();
        }
        public string PractitionerId { get; set; }
        public DateTime GeneratedOn { get; set; }

        public IList<QrCodeModelDto> QrCodeModelDtos { get; set; }
    }

    public class QrCodeModelDto
    { 
        public string Skill { get; set; }
        public string Credential { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime? InactiveDate { get; set; }
        public Guid QrCode { get; set; }
        public bool IsDeceased { get; set; }
        public bool AlowVerifyOnline { get; set; }
    }
}

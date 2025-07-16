using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using Ncms.Contracts.Models.Person;

namespace Ncms.Contracts.Models.Application
{
    public class CredentialExportModel
    {
        public int NaatiNumber { get; set; }
        public string PractitionerNumber { get; set; }
        public string Title { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Address { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
        public string PrimaryEmail { get; set; }
        public int CredentialId { get; set; }
        public string ApplicationType { get; set; }
        public string InternalName { get; set; }
        public string ExternalName { get; set; }
        public string Language1 { get; set; }
        public string Language1Code { get; set; }
        public string Language1Group { get; set; }
        public string Language2 { get; set; }
        public string Language2Code { get; set; }
        public string Language2Group { get; set; }
        public string DirectionDisplayName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public DateTime StatusChangeDate { get; set; }
        public DateTime ExportedDate { get; set; }
    }
}
using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal;

namespace Ncms.Contracts.Models.Panel
{
    public class PanelMembershipModel
    {
        public int? PanelMembershipId { get; set; }
        public int PersonId { get; set; }
        public int NaatiNumber { get; set; }
        public string PersonName { get; set; }
        public int PanelId { get; set; }
        public string PanelName { get; set; }
        public int PanelRoleId { get; set; }
        public string PanelRole { get; set; }
        public bool IsExaminerRole { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool HasUnavailability { get; set; }
        public bool HasMarkingRequests { get; set; }
        public bool HasMaterialRequests { get; set; }
        public int[] CredentialTypeIds { get; set; }
        public int[] MaterialCredentialTypeIds { get; set; }
        public int[] CoordinatorCredentialTypeIds { get; set; }
        public IList<ExaminerUnavailableContract> UnAvailableExaminers { get; set; }
        public int InProgress { get; set; }
        public int Overdue { get; set; }

    }

    public class ExaminerUnavailability
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class MarkingRequest
    {
        public int AttendanceId { get; set; }
        public int NaatiNumber { get; set; }
        public string ApplicantName { get; set; }
        public string Type { get; set; }
        public string Language { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public string Direction { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime TestDate { get; set; }
    }

    public class MaterialRequest
    {
        public int TestMaterialID { get; set; }
        public int JobExaminerID { get; set; }
        public int JobID { get; set; }
        public string Language { get; set; }
        public string Category { get; set; }
        public string Direction { get; set; }
        public string Level { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? DateReceived { get; set; }
        public decimal? Cost { get; set; }
        public bool Approved { get; set; }
    }
}

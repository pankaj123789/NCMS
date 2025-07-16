using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using Ncms.Contracts.Models;

namespace Ncms.Contracts
{
    public interface IPayrollService
    {
        IEnumerable<ProductSpecificationLookupModel> GetProductSpecificationsForMarkingClaimsLookupList();
        void ValidateMarkingForPayroll(IEnumerable<int> jobExaminerIds);
        void UnvalidateMarkingForPayroll(int jobExaminerId);
        IEnumerable<MarkingForPayrollModel> GetMarkingsForValidation();
        IEnumerable<MarkingForPayrollModel> GetMarkingsForNewPayroll();
        IEnumerable<MarkingForPayrollModel> GetMarkingsForInProgressPayroll();
        IEnumerable<MarkingForPayrollModel> GetMarkingsForPreviousPayroll(PreviousMarkingFilterForPayrollModel filter);
        void CreatePayroll(IEnumerable<int> jobExaminerIds);
        bool IsPayrollInProgress();
        GenericResponse<bool> AssignInvoiceNumber(JobExaminerPayrollModel model);
        GenericResponse<bool> RemoveExaminerFromPayroll(JobExaminerPayrollModel model);
        void FinalisePayroll(int payrollId);
    }


    public class PreviousMarkingFilterForPayrollModel
    {
        public DateTime? from { get; set; }
        public DateTime? to { get; set; }
    }
    public class GlCodeModel
    {
        public int GLCodeId { get; set; }
        public string Code { get; set; }
    }

    public class ProductSpecificationDetailsModel
    {
        public int Id { get; set; }
        public string GlCode { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public virtual string Description { get; set; }
        public decimal CostPerUnit { get; set; }
        public bool Inactive { get; set; }
        public bool GstApplies { get; set; }
    }
    public class MarkingForPayrollModel
    {
        public int JobExaminerId { get; set; }
        public string Examiner { get; set; }
        public int ExaminerPersonId { get; set; }
        public int ExaminerEntityId { get; set; }
        public int ExaminerNaatiNumber { get; set; }
        public string ExaminerAccountNumber { get; set; }
        public int ProductSpecificationId { get; set; }
        public string ProductSpecificationCode { get; set; }
        public string GlCode { get; set; }
        public decimal ExaminerCost { get; set; }
        public DateTime ResultReceivedDate { get; set; }
        public string AccountingReference { get; set; }
        public int? PayrollId { get; set; }
        public DateTime? PayrollModifiedDate { get; set; }
        public string PayrollModifiedUser { get; set; }
        public int TestAttendanceId { get; set; }
        public string Language { get; set; }
        public string ValidatedUser { get; set; }
        public DateTime? ValidatedDate { get; set; }
        public string Office { get; set; }
        public string ProductSpecificationChangedUser { get; set; }
        public string TestType { get; set; }
        public bool PaidReviewer { get; set; }
        public bool Supplementary { get; set; }
    }

    public class JobExaminerPayrollModel
    {
        public int PayrollId { get; set; }
        public IEnumerable<int> JobExaminerIds { get; set; }
        public string AccountingReference { get; set; }
    }
}


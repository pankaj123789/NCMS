using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    
    public interface IPayrollQueryService : IQueryService
    {
        
        GetProductSpecificationLookupListResponse GetProductSpecificationsForProductType(int productTypeId);

        
        GetMarkingsForPayrollResponse GetMarkingsForPayroll(GetMarkingsForPayrollRequest request);

        
        void ValidateMarkingForPayroll(ValidateMarkingForPayrollRequest request);

        
        void UnvalidateMarkingForPayroll(int jobExaminerId);

        
        CreatePayrollResponse CreatePayroll(CreatePayrollRequest request);

        GetProductSpecificationLookupListResponse GetProductSpecificationByCode(string code);

        bool IsPayrollInProgress();

        
        AssignPayrollAccountingReferenceResponse AssignPayrollAccountingReference(AssignPayrollAccountingReferenceRequest request);

        
        RemoveExaminerFromPayrollResponse RemoveExaminerFromPayroll(RemoveExaminerFromPayrollRequest request);

        
        void SetPayrollStatus(SetPayrollStatusRequest request);
    }
}

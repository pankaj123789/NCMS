using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;

namespace MyNaati.Contracts.BackOffice
{
    
    public interface ILogbookService : IInterceptableservice
    {
        
        IEnumerable<ProfessionalDevelopmentRequirementResponse> GetProfessionalDevelopmentRequirements(int categoryId);
        
        IEnumerable<ProfessionalDevelopmentCategoryResponse> GetProfessionalDevelopmentCategories();
        
        ProfessionalDevelopmentCategoryRequirementListResponse GetProfessionalDevelopmentCategoriesRequirements(
          int sectionId);
        
        IEnumerable<ProfessionalDevelopmentActivityDto> GetProfessionalDevelopmentActivities(int naatiNumber, int certificationPeriodId);
        
        GetCredentialResponse GetCredentials(int naatiNumber);
        
        void DeleteWorkPractice(int id);
        
        void DeleteProfessionalDevelopmentCategoryRequirement(int id);
        
        void DeleteProfessionalDevelopmentActivity(int id);
        
        void DeleteProfessionalDevelopmentCategory(int id);
        
        void DeleteProfessionalDevelopmentRequirement(int id);
        
        void DeleteProfessionalDevelopmentSection(int id);
        
        void DeleteWorkPracticeAttachment(int id);
        
        void DeleteProfessionalDevelopmentActivityAttachment(int id);
        
        IEnumerable<WorkPracticeResponse> GetWorkPractices(int id, int naatiNumber, int certificationPeriodId);
        
        ProfessionalDevelopmentCategoryRequirementResponse GetProfessionalDevelopmentCategoryRequirement(int id);
        
        ProfessionalDevelopmentActivityDto GetProfessionalDevelopmentActivity(int id);
        
        ProfessionalDevelopmentCategoryResponse GetProfessionalDevelopmentCategory(int id);
        
        ProfessionalDevelopmentRequirementResponse GetProfessionalDevelopmentRequirement(int id);
        
        ProfessionalDevelopmentSectionResponse GetProfessionalDevelopmentSection(int id);
        
        IEnumerable< WorkPracticeAttachmentResponse> GetWorkPracticeAttachments(int workPracticeId);

        
        IEnumerable<ProfessionalDevelopmentActivityAttachmentDto> GetProfessionalDevelopmentActivityAttachments(
            int activityId);
        
        CreateOrUpdateResponse CreateOrUpdateWorkPractice(WorkPracticeRequest model);
        
        void CreateOrUpdateProfessionalDevelopmentCategoryRequirement(ProfessionalDevelopmentCategoryRequirementRequest model);
        
        CreateOrUpdateResponse CreateOrUpdateProfessionalDevelopmentActivity(ProfessionalDevelopmentActivityRequest model, int naatiNumber);
        
        void CreateOrUpdateProfessionalDevelopmentCategory(ProfessionalDevelopmentCategoryRequest model);
        
        void CreateOrUpdateProfessionalDevelopmentRequirement(ProfessionalDevelopmentRequirementRequest model);
        
        void CreateOrUpdateProfessionalDevelopmentSection(ProfessionalDevelopmentSectionRequest model);
        
        AttachmentResponse CreateOrUpdateWorkPracticeAttachment(WorkPracticeAttachmentRequest model);
        
        AttachmentResponse CreateOrUpdateProfessionalDevelopmentActivityAttachment(ProfessionalDevelopmentActivityAttachmentRequest model);
        
        GetFileResponse GetAttachment(GetFileRequest request);
        
        PdActivityPoints GetProfessionalActivityPoints(int naatiNumber, int certificationPeriodId);

        
        IEnumerable<CertificationPeriodDetailsDto> GetCertificationPeriodDetails(int naatiNumber);

        
        IEnumerable<CredentialCertificationPeriodDetailsDto> GetCredentialCertificationPeriodDetails(int naatiNumber, int credentialId);
        
        IEnumerable<CertificationPeriodRequests> GetCertificationPeriodRequests(int naatiNumber);
        
        WorkPracticeCredentialDto GetCertificationPeriodCredential(int naatiNumber, int certificationPeriodId,int credentialId);
        
        RecertificationStatusResponse GetCredentialRecertificationStatus(int credentialId);
        
        bool IsValidWorkPracticeAttachment(int workPracticeAttachmentId, int naatiNumber);
        
        bool IsValidActivityAttachment(int activityAttachmentId, int naatiNumber);
        
        bool IsValidWorkPracticeStoredFieldId(int storedFieldId, int naatiNumber);
        
        bool IsValidActivityStoredFieldId(int storedFieldId, int naatiNumber);
        
        bool IsValidateWorkPractice(int workPracticeId, int naatiNumber);

        
        bool IsValidActivity(int activityId, int naatiNumber);
    }

    
    public class RecertificationStatusResponse
    {
        
        public int StatusId { get; set; }
    }

    
    public class CertificationPeriodRequest
    {
        
        public string Skill { get; set; }

        
        public string ExternalName { get; set; }
    }

    
    public class CertificationPeriodRequests
    {
        
        public  int CertificationPeriodId { get; set; }
        
        public IEnumerable<CertificationPeriodRequest> Requests { get; set; }
    }






}

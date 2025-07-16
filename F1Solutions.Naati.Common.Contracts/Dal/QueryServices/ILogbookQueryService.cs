using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    
    public interface ILogbookQueryService : IQueryService
    {
        
        IEnumerable<ProfessionalDevelopmentRequirementResponse> GetProfessionalDevelopmentRequirements(int categoryId);
        
        IEnumerable<ProfessionalDevelopmentCategoryResponse> GetProfessionalDevelopmentCategories();
        
        ProfessionalDevelopmentCategoryRequirementListResponse GetProfessionalDevelopmentCategoriesRequirements(
            int sectionId);

        
        IEnumerable<CredentialsDetailsDto> GetCertificationPeriodCredentials(int certificationPeriodId);

        
        IEnumerable<CredentialsDetailsDto> GetSubmittedRecertificationApplicationCredentials(int credentialApplicationId);

        
        GetCredentialResponse GetCredentials(int naatiNumber);

        
        IEnumerable<CertificationPeriodRequestDto> GetSubmittedRecertificationCredentialRequests(GetCerfiationPeriodRequest request);

        
        IEnumerable<ProfessionalDevelopmentActivityDto> GetRecertificationAtivitiesForApplication(int applicationId);

        
        ProfessionalDevelopmentActivityResponse GetProfessionalDevelopmentActivities(GetActivitiesRequest request);


        
        void DeleteWorkPractice(int id);

        
        void DeleteProfessionalDevelopmentCategoryRequirement(int id);

        
        void DeleteProfessionalDevelopmentActivity(int id);

        
        void DetachProfessionalDevelopmentActivity(int activityId, int credentialApplicationId);

        
        void AttachProfessionalDevelopmentActivity(int activityId, int credentialApplicationId);

        
        void DeleteProfessionalDevelopmentCategory(int id);

        
        void DeleteProfessionalDevelopmentRequirement(int id);

        
        void DeleteProfessionalDevelopmentSection(int id);

        
        void DeleteWorkPracticeAttachment(int id);

        
        void DeleteProfessionalDevelopmentActivityAttachment(int id);

        
        IEnumerable<WorkPracticeDetails> GetWorkPractices(int credentialId);

        
        IEnumerable<WorkPracticeDetails> GetWorkPracticesForRecertificationApplication(int credentialId,
            int credentialApplicationId);

        
        ProfessionalDevelopmentCategoryRequirementResponse GetProfessionalDevelopmentCategoryRequirement(int id);

        
        ProfessionalDevelopmentActivityDto GetProfessionalDevelopmentActivity(int id);

        
        ProfessionalDevelopmentCategoryResponse GetProfessionalDevelopmentCategory(int id);

        
        ProfessionalDevelopmentRequirementResponse GetProfessionalDevelopmentRequirement(int id);

        
        ProfessionalDevelopmentSectionResponse GetProfessionalDevelopmentSection(int id);

        
        IEnumerable<WorkPracticeAttachmentResponse> GetWorkPracticeAttachments(int workPracticeId);

        
        ProfessionalDevelopmentActivityAttachmentResponse GetProfessionalDevelopmentActivityAttachments(
            int activityId);

        
        CreateOrUpdateResponse CreateOrUpdateWorkPractice(WorkPracticeRequest model);

        
        void CreateOrUpdateProfessionalDevelopmentCategoryRequirement(ProfessionalDevelopmentCategoryRequirementRequest model);

        
        CreateOrUpdateResponse CreateOrUpdateProfessionalDevelopmentActivity(ProfessionalDevelopmentActivityRequest model, int naatiNumber);

        
        void CreateOrUpdateProfessionalDevelopmentCategory(ProfessionalDevelopmentCategoryRequest model);

        
        void CreateOrUpdateProfessionalDevelopmentRequirement(ProfessionalDevelopmentRequirementRequest model);

        
        void CreateOrUpdateProfessionalDevelopmentSection(ProfessionalDevelopmentSectionRequest model);

        
        AttachmentResponse CreateOrUpdateWorkPracticeAttachment(WorkPracticeAttachmentRequest model);

        
        AttachmentResponse CreateOrUpdateProfessionalDevelopmentActivityAttachment(ProfessionalDevelopmentActivityAttachmentRequest model);

        
        ActivityPointsConfigurationResponse GetActivityPointsConfiguration();

        
        CredentialPointsResponse GetCredentialPoints(CredentialPointsRequest request);

        
        void AttachWorkPractice(int workPracticeId, int credentialApplicationId, int credentialId);

        
        void DetachWorkPractice(int id);

        
        RecertificationDto GetSubmittedRecertificationApplicationForPeriod(int certificationPeriodId);

        
        bool IsValidateWorkPracticeAttachment(int workPracticeAttachmentId, int naatiNumber);

        
        bool IsValidActivityAttachment(int activityAttachmentId, int naatiNumber);

        
        bool IsValidWorkPracticeStoredFieldId(int storedFieldId, int naatiNumber);

        
        bool IsValidActivityStoredFieldId(int storedFieldId, int naatiNumber);

        
        bool IsValidateWorkPractice(int workPracticeId, int naatiNumber);

        
        bool IsValidActivity(int activityId, int naatiNumber);
    }
}
       
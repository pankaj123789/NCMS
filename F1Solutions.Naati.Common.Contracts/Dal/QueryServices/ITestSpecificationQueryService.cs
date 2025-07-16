using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;
using F1Solutions.Naati.Common.Dal.Domain;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    
    public interface ITestSpecificationQueryService : IQueryService
    {
        
        TestSpecificationResponse Get(TestSpecificationRequest request);

        
        GenericResponse<string> Edit(TestSpecificationRequest request, GenericResponse<string> response);

        
        AttachmentResponse CreateOrReplaceAttachment(TestSpecificationAttachmentRequest request);

        
        void DeleteAttachment(TestSpecificationAttachmentRequest request);

        
        TestSpecificationAttachmentResponse GetAttachments(TestSpecificationRequest request);

        
        TestSpecificationAttachmentResponse GetExaminerAttachmentsForSitting(int testSittingId);

        
        TestComponentResponse GetTestComponentsBySpecificationId(TestSpecificationRequest request);
        bool CanUpload(TestSpecificationRequest request);



        TestSpecificationResponse GetTestSpecificationByCredentialTypeId(SpecificationByCredentialTypeRequest request);
        TestSpecification GetTestSpecificationById(int testSpecificationId);



        GetDocumentTypesForApplicationTypeResponse GetDocumentTypesForTestSpecificationType();

        
        GetQuestionPassRulesResponse GetQuestionPassRules(GetRubricConfigurationRequest request);

        
        SaveRubricConfigurationResponse SaveQuestionPassRules(SaveQuestionPassRulesRequest request);

        
        GetTestBandRulesResponse GetTestBandRules(GetRubricConfigurationRequest getRubricConfigurationRequest);

        
        SaveRubricConfigurationResponse SaveTestBandRules(SaveTestBandRulesRequest saveTestBandRulesRequest);

        
        GetTestQuestionRulesResponse GetTestQuestionRules(GetRubricConfigurationRequest getRubricConfigurationRequest);

        
        SaveRubricConfigurationResponse SaveTestQuestionRules(SaveTestQuestionRulesRequest saveTestQuestionRulesRequest);

        
        GetRubricMarkingBandResponse GetMarkingBand(GetMarkingBandRequest getMarkingBandRequest);

        
        UpdateRubricMarkingBandResponse UpdateMarkingBand(UpdateMarkingBandRequest saveMarkingBandRequest);
        
        GetRubricConfigurationResponse GetRubricConfiguration(GetRubricConfigurationRequest request);

        /// <summary>
        /// Creates a Test Specification based on CredentialRequestId and Title
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Generic Response containing created id</returns>
        GenericResponse<int> AddTestSpecifiction(AddTestSpecificationRequest request);

        /// <summary>
        /// Returns list of TestSessions to be alerted to NAATI in batch job
        /// where TestMaterials have not been fully allocated within required time period
        /// </summary>
        /// <returns></returns>
        GenericResponse<List<int>> GetTestSessionIdsWhereMaterialsNotYetFullyAllocated();
    }
}


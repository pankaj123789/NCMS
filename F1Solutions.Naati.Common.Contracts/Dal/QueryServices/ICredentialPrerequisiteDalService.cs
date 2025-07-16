using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    public interface ICredentialPrerequisiteDalService
    {
        /// <summary>
        /// Gets all CredentialPrerequisites from the database
        /// </summary>
        /// <returns>IEnumerable of domain model CredentialPrerequisite</returns>
        IEnumerable<CredentialPrerequisite> GetCredentialPrerequisites();
        /// <summary>
        /// Calls upon the private CreateApplication function to create the applications for upserting for the
        /// createPrerequisiteApplications Action
        /// </summary>
        /// <param name="createPrerequisiteApplicationsDalModel"></param>
        /// <returns>PrerequisiteApplicationDalModel</returns>
        PrerequisiteApplicationDalModel CreateApplicationAndCredentialRequest(CreatePrerequisiteApplicationsDalModel createPrerequisiteApplicationsDalModel);
        /// <summary>
        /// Calls upon the Application Query Service UpsertApplication method to upsert the newly created prerequisite application to the database
        /// </summary>
        /// <param name="upsertCredentialApplicationRequest"></param>
        /// <returns>UpsertApplicationResponse</returns>
        UpsertApplicationResponse UpsertApplicationAndCredentialRequest(UpsertCredentialApplicationRequest upsertCredentialApplicationRequest);
        /// <summary>
        /// Retrieves the Prerequisite Applications for the given Application Id by use of the PrerequisiteApplications stored procedure
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns>PrerequisiteApplicationsResult</returns>
        GenericResponse<PrerequisiteApplicationsResult> GetPrerequisiteApplications(int applicationId);
        /// <summary>
        /// Retrieves the Prerequisite Requests with Applications that can be null for the given Credential Request Id by use of the PrerequisiteApplicationsNullableApplications stored procedure
        /// </summary>
        /// <param name="credentialRequestId"></param>
        /// <returns>PrerequisiteApplicationsNullableApplicationsResult</returns>
        GenericResponse<PrerequisiteApplicationsNullableApplicationsResult> GetPrerequisiteApplicationsNullableApplications(int credentialRequestId);
        /// <summary>
        /// Retrieves the Prerequisite Summary for the given Credential Request Id by use of the PrerequisiteSummary stored procedure 
        /// </summary>
        /// <param name="credentialRequestId"></param>
        /// <returns>GenericResponse&#60;PrerequisiteSummaryResult&#62;</returns>
        GenericResponse<PrerequisiteSummaryResult> GetPrerequisiteSummary(int credentialRequestId);

        /// <summary>
        /// Gets the details of a credential request for the given credential request id
        /// </summary>
        /// <param name="credentialRequestId"></param>
        /// <returns>Details of a Credential Request</returns>        
        GenericResponse<CredentialRequestDetails> GetCredentialRequestDetails(int credentialRequestId);

        /// <summary>
        /// Gets a list of the exemptions a person has based off the details of a credential request
        /// </summary>
        /// <param name="credentialRequestDetails"></param>
        /// <returns>A list of credential prerequisite exemptions</returns>
        GenericResponse<List<CredentialPrerequisiteExemptionDto>> GetActiveCredentialPrerequisiteExemptions(CredentialRequestDetails credentialRequestDetails);

        /// <summary>
        /// Gets a list of the exemptions a person has based off the details of a credential request
        /// </summary>
        /// <param name="personId"></param>
        /// <returns>A list of credential prerequisite exemptions</returns>
        GenericResponse<List<CredentialPrerequisiteExemptionDto>> GetAllCredentialPrerequisiteExemptions(int naatiNumber);

        /// <summary>
        /// Gets a list of the exemptions a person has based off the details of a credential request
        /// </summary>
        /// <param name="credentialRequestDetails"></param>
        /// <returns></returns>
        GenericResponse<List<CredentialPrerequisiteDetails>> GetPrerequisitesForCredentialRequest(CredentialRequestDetails credentialRequestDetails);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="credentialPrerequisiteExemptionDto"></param>
        /// <returns></returns>
        GenericResponse<bool> SaveOrUpdateCredentialPrerequisiteExemptions(CredentialPrerequisiteExemptionDto credentialPrerequisiteExemptionDto);
        /// <summary>
        /// Gets the skill for the prerequisite exemption
        /// </summary>
        /// <param name="prerequisiteApplicationTypeId"></param>
        /// <param name="prerequisiteCredentialTypeId"></param>
        /// <param name="parentCredentialRequestId"></param>
        /// <returns>Skill Id for the prerequisite credential relating to the exemption</returns>
        GenericResponse<SkillDetails> GetSkillForExemption(int prerequisiteApplicationTypeId, int prerequisiteCredentialTypeId, int parentCredentialRequestId);

        GenericResponse<OnHoldCredentialChildParameters> GetChildParamsForRelatedCredentialsOnHold(int childCredentialRequestId);

        GenericResponse<List<OnHoldCredentialParentParameters>> GetParentParamsForRelatedCredentialsOnHold(OnHoldCredentialChildParameters childParams);

        GenericResponse<bool> CheckParentCredentialsOnHoldElligibleToIssue(OnHoldCredentialParentParameters parentParams);
    }

    public class OnHoldCredentialChildParameters
    {
        public int CredentialRequestId { get; set; }
        public int CredentialTypeId { get; set; }
        public string CredentialTypeName { get; set; }
        public int PersonId { get; set; }
        public int SkillId { get; set; }
        public string SkillDisplayName { get; set; }
        public int Language1Id { get; set; }
        public int Language2Id { get; set; }
        public int CredentialApplicationId { get; set; }
        public int CredentialApplicationStatusTypeId { get; set; }
        public string CredentialApplicationStatusTypeDisplayName { get; set; }
        public string CredentialApplicationTypeDisplayName { get; set; }
    }

    public class OnHoldCredentialParentParameters
    {
        public int CredentialRequestId { get; set; }
        public int CredentialTypeId { get; set; }
        public string CredentialTypeName { get; set; }
        public string SkillDisplayName { get; set; }
        public int CredentialRequestStatusTypeId { get; set; }
        public string CredentialRequestStatusTypeDisplayName { get; set; }
        public int CredentialApplicationTypeId { get; set; }
        public OnHoldCredentialChildParameters ChildParameters { get; set; }
        public int CredentialApplicationId { get; set; }
        public int CredentialApplicationStatusTypeId { get; set; }
        public string CredentialApplicationStatusTypeDisplayName { get; set; }
        public string CredentialApplicationTypeDisplayName { get; set; }
    }
}
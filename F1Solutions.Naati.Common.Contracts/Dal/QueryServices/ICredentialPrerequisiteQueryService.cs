using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Dal.Domain;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    public interface ICredentialPrerequisiteQueryService : IQueryService
    {
        IEnumerable<CredentialPrerequisite> GetCredentialPrerequisites();
        PrerequisiteSummaryResult GetPreReqsForCredRequest(int credentialRequestId);

        PrerequisiteSummaryResult GetPrerequisiteSummary(int credentialRequestId);
        /// <summary>
        /// Used to determine Cred Req type from Wizard 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        int GetCredentialRequestTypeFromName(string name);

        /// <summary>
        /// Gets a dictionary of credentialapplication ids containing a list of mandatory fields for each
        /// </summary>
        /// <param name="parentCredentialRequestId"></param>
        /// <returns></returns>
        GenericResponse<Dictionary<int, List<string>>> GetCredentialApplicationTypeListWithMandatoryFields(int parentCredentialRequestId);

        /// <summary>
        /// Validating mandatory fields for raising a new Applicaiton based on parentApplication
        /// This gets the fields already raised
        /// </summary>
        /// <param name="credentialRequestId"></param>
        /// <returns></returns>
        GenericResponse<IEnumerable<string>> GetExistingApplicationFieldsForCredentialRequest(int credentialRequestId);

        /// <summary>
        /// gets specific mandatory fields for the applicationType
        /// </summary>
        /// <param name="credentialApplicationTypeId"></param>
        /// <returns></returns>
        GenericResponse<List<string>> GetMandatoryFieldsForApplicationType(int credentialApplicationTypeId);

        /// <summary>
        /// gets specific mandatory documents for the applicationType
        /// </summary>
        /// <param name="credentialApplicationTypeId"></param>
        /// <returns></returns>
        GenericResponse<List<string>> GetMandatoryDocumentsForApplicationType(int credentialApplicationTypeId);

        /// <summary>
        /// Retrieves all DocumentTypes for a supplied CredentialRequestId
        /// </summary>
        /// <param name="parentCredentialRequestId"></param>
        /// <returns></returns>
        GenericResponse<IEnumerable<string>> GetExistingDocumentTypesForCredentialRequest(int parentCredentialRequestId);
    }
}

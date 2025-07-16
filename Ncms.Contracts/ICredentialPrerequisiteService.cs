using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Dal.Domain;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.CredentialPrerequisite;
using Ncms.Contracts.Models.User;
using System.Collections.Generic;

namespace Ncms.Contracts
{
    public interface ICredentialPrerequisiteService
    {
        /// <summary>
        /// Calls the Credential Prerequisite Dal Service to get all CredentialPrerequisites from the database
        /// </summary>
        /// <returns>IEnumerable of type CredentialPrerequisite</returns>
        IEnumerable<CredentialPrerequisite> GetCredentialPrerequisites();
        /// <summary>
        /// Gets the Id for all the passed in Credential Prerequisites
        /// </summary>
        /// <param name="credentialPrerequisites"</param>
        /// <returns>IEnumerable of type int</returns>
        IEnumerable<int> GetCredentialPrerequisiteIds(IEnumerable<CredentialPrerequisite> credentialPrerequisites);

        PrerequisiteSummaryResult GetPreReqsForCredRequest(int credentialRequestId);

        /// <summary>
        /// Checks the mandatory fields for a given Credential Request. If the mandatory fields are not met then return the missing field as an "error" to the UI
        /// </summary>
        /// <param name="createPrerequisiteRequest"</param>
        /// <returns>GenericResponse of type ValidatePrerequisiteResponse</returns>
        GenericResponse<ValidatePrerequisiteResponse> ValidateMandatoryFields(CreatePrerequisiteRequest createPrerequisiteRequest);

        /// <summary>
        /// Checks the mandatory documents for a given Credential Request. If the mandatory documents are not met then return the missing field as an "error" to the UI
        /// </summary>
        /// <param name="createPrerequisiteRequest"</param>
        /// <returns>GenericResponse of type ValidatePrerequisiteResponse</returns>
        GenericResponse<ValidatePrerequisiteResponse> ValidateMandatoryDocuments(CreatePrerequisiteRequest createPrerequisiteRequest);

        /// <summary>
        /// Calls the CredentialPrerequisiteDalService to get the prerequisite summary for a given credential request Id
        /// </summary>
        /// <param name="credentialRequestId"</param>
        /// <returns>GenericResponse of type PrerequisiteSummaryResult</returns>
        GenericResponse<PrerequisiteSummaryResult> GetPrerequisiteSummary(int credentialRequestId);
        /// <summary>
        /// Gets a list of the credential exemptions for the given credential request id
        /// </summary>
        /// <param name="credentialRequestId"></param>
        /// <returns>List of Credential Prerequisite Exemptions</returns>
        GenericResponse<List<CredentialPrerequisiteExemptionModel>> GetCredentialPrerequisiteExemptions(int credentialRequestId);

        /// <summary>
        /// Get all Exceptions for a Person
        /// </summary>
        /// <param name="naatiNumber"></param>
        /// <returns>List of Credential Prerequisite Exemptions</returns>
        GenericResponse<List<CredentialPrerequisiteExemptionModel>> GetExemptions(int naatiNumber);

        /// <summary>
        /// Handles whether exemptions should be saved/ updated and calls the db to save/ update these exemptions.
        /// </summary>
        /// <param name="prerequisiteExemptionRequests"></param>
        /// <returns>Generic Response</returns>
        GenericResponse<bool> HandlePrerequisiteExemptions(PrerequisiteExemptionRequest prerequisiteExemptionRequests, UserModel currentUser);
        /// <summary>
        /// Gets all levels of parents that are on hold for a given credential request id
        /// </summary>
        /// <param name="credentialRequestId"></param>
        /// <param name="parentsOnHoldToBeIssued"></param>
        /// <returns>List of On Hold Credential Models</returns>
        GenericResponse<IEnumerable<OnHoldCredentialsToBeIssuedModel>> GetRelatedCredentialIdsOnHold(int credentialRequestId, List<OnHoldCredentialsToBeIssuedModel> parentsOnHoldToBeIssued = null);
    }
}
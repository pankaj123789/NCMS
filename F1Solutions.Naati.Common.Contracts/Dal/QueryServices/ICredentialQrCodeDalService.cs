using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Bl.VerifyPractitioner;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Dal.Domain;
using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    public interface ICredentialQrCodeDalService:IQueryService
    {
        /// <summary>
        /// Pass in a credential ID
        /// If Credential is not current then it will error
        /// If there is a record for the current day already it will return
        /// If not then a new record is created and returned
        /// </summary>
        /// <param name="CredentialId"></param>
        /// <returns></returns>
        GenericResponse<CredentialQrCodeDto> GetCredentialQRCodeForStampAndId(int credentialId);
        /// <summary>
        /// used by the API to verify the QRCode
        /// If the code doesnt ecist then return error
        /// If it does return the details (yet to be determined)
        /// </summary>
        /// <param name="QrCodeGuid"></param>
        /// <returns></returns>
        GenericResponse<CredentialQrCodeDto> VerifyCredentialQRCode(Guid qrCodeGuid);
        /// <summary>
        /// Gets the credential type catergory display name from the database using a credential type ID
        /// </summary>
        /// <param name="credentialTypeId"></param>
        /// <returns>Display Name of the Credential Type Catergory</returns>
        GenericResponse<string> GetCredentialCatergoryByCredentialTypeExternalName(string credentialTypeExternalName);
        /// <summary>
        /// Gets a persons MFA code
        /// </summary>
        /// <param name="naatiNumber"></param>
        /// <returns>A persons mfa code</returns>
        GenericResponse<Person> GetPerson(int naatiNumber);

        /// <summary>
        /// This gets the PractitionerId from the QrCode Guid
        /// </summary>
        /// <param name="qrCode"></param>
        /// <returns></returns>
        GenericResponse<QrCodeVerificationModelDto> GetPractitionerVerificationModelFromQrCode(Guid qrCode);
        /// <summary>
        /// Gets true or false depending on the deceased status of a person for the given practitioner Id
        /// </summary>
        /// <param name="practitionerId"></param>
        /// <returns>True if deceased, false if not.</returns>
        GenericResponse<bool> GetIsDeceasedFromPractitionerId(string practitionerId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="credentialId"></param>
        /// <param name="currentUserNaatiNumber"></param>
        /// <returns></returns>
        GenericResponse<bool> DoesCredentialBelongToUser(int credentialId, int currentUserNaatiNumber);
    }
}